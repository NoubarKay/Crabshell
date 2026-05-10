using Crabshell.Core.Attributes;
using Crabshell.Core.Registry;
using Crabshell.Core.Schema;
using Microsoft.EntityFrameworkCore;

namespace Crabshell.Data.Schema;

public class SchemaDiffService(CrabshellDbContext db, CollectionRegistry registry, IDatabaseDialect dialect) : ISchemaDiffService
{
    public async Task ApplyDiffAsync()
    {
        // Pass 1: ensure all tables and columns exist before adding FK constraints
        foreach (var collection in registry.GetAll())
        {
            var existingColumns = await GetExistingColumnsAsync(collection.Slug);

            if (!existingColumns.Any())
                await CreateTableAsync(collection);
            else
            {
                foreach (var field in collection.Fields)
                {
                    if (!existingColumns.Contains(field.ColumnName))
                        await AddColumnAsync(collection.Slug, field);
                }

                if (!existingColumns.Contains("is_deleted"))
                    await db.Database.ExecuteSqlRawAsync(
                        dialect.AddColumnIfNotExists(collection.Slug, $"\"is_deleted\" {dialect.BoolType} NOT NULL DEFAULT false"));

                if (!existingColumns.Contains("deleted_at"))
                    await db.Database.ExecuteSqlRawAsync(
                        dialect.AddColumnIfNotExists(collection.Slug, $"\"deleted_at\" {dialect.TimestampType} NULL"));
            }
        }

        // Pass 2: ensure FK constraints exist now that all tables are guaranteed present
        foreach (var collection in registry.GetAll())
        {
            foreach (var field in collection.Fields.Where(f =>
                f.FieldType == FieldType.Relationship && f.RelationshipSettings is not null))
            {
                await EnsureForeignKeyAsync(collection.Slug, field);
            }
        }
    }

    private async Task<HashSet<string>> GetExistingColumnsAsync(string tableName)
    {
        var columns = new HashSet<string>();
        var conn = db.Database.GetDbConnection();
        var wasOpen = conn.State == System.Data.ConnectionState.Open;

        if (!wasOpen) await conn.OpenAsync();
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = dialect.GetExistingColumnsQuery(tableName);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                columns.Add(reader.GetString(dialect.ColumnNameResultIndex));
        }
        finally
        {
            if (!wasOpen) await conn.CloseAsync();
        }
        return columns;
    }
    
    private async Task CreateTableAsync(CollectionMeta collection)
    {
        var columns = new List<string>
        {
            $"id {dialect.UuidType} NOT NULL DEFAULT {dialect.NewUuid()} PRIMARY KEY",
            $"created_at {dialect.TimestampType} NOT NULL DEFAULT {dialect.Now()}",
            $"updated_at {dialect.TimestampType} NOT NULL DEFAULT {dialect.Now()}",
            $"is_deleted {dialect.BoolType} NOT NULL DEFAULT false",
            $"deleted_at {dialect.TimestampType} NULL"
        };

        foreach (var field in collection.Fields)
            columns.Add(BuildColumnDdl(field));

        var sql = $"""
                   CREATE TABLE IF NOT EXISTS "{collection.Slug}" (
                       {string.Join(",\n    ", columns)}
                   );
                   """;

        await db.Database.ExecuteSqlRawAsync(sql);
    }

    private async Task AddColumnAsync(string tableName, FieldMeta field)
    {
        var columnDdl = BuildColumnDdl(field);
        var sql = dialect.AddColumnIfNotExists(tableName, columnDdl);
        await db.Database.ExecuteSqlRawAsync(sql);
    }

    private string BuildColumnDdl(FieldMeta field)
    {
        var columnType = dialect.GetColumnType(field);

        // Bool is always NOT NULL DEFAULT false — CLR bool is never nullable
        if (field.FieldType == FieldType.Bool)
            return $"\"{field.ColumnName}\" {columnType} NOT NULL DEFAULT false";

        // Non-nullable value types (int, decimal, enum, Guid, etc.) are always NOT NULL
        var isNonNullableValueType = field.ClrType.IsValueType &&
                                     Nullable.GetUnderlyingType(field.ClrType) is null;

        if (isNonNullableValueType || field.Required)
        {
            var def = GetDefaultValue(field);
            var defClause = def is not null ? $" DEFAULT {def}" : "";
            return $"\"{field.ColumnName}\" {columnType} NOT NULL{defClause}";
        }

        return $"\"{field.ColumnName}\" {columnType} NULL";
    }

    private string? GetDefaultValue(FieldMeta field) => field.FieldType switch
    {
        FieldType.Relationship => null,
        FieldType.DateTime     => dialect.Now(),
        FieldType.Number       => "0",
        FieldType.Bool         => "false",
        FieldType.Select       => IsEnumBacked(field) ? "0" : "''",
        _                      => "''"   // Text, RichText
    };

    private static bool IsEnumBacked(FieldMeta field)
    {
        var t = Nullable.GetUnderlyingType(field.ClrType) ?? field.ClrType;
        return t.IsEnum;
    }

    private async Task EnsureForeignKeyAsync(string tableName, FieldMeta field)
    {
        var conn = db.Database.GetDbConnection();
        var wasOpen = conn.State == System.Data.ConnectionState.Open;
        if (!wasOpen) await conn.OpenAsync();

        try
        {
            bool exists;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT COUNT(*)
                    FROM information_schema.table_constraints tc
                    JOIN information_schema.key_column_usage kcu
                        ON tc.constraint_name = kcu.constraint_name
                        AND tc.table_schema = kcu.table_schema
                    WHERE tc.constraint_type = 'FOREIGN KEY'
                        AND tc.table_name = @table
                        AND kcu.column_name = @column";

                var p1 = cmd.CreateParameter(); p1.ParameterName = "@table"; p1.Value = tableName;
                var p2 = cmd.CreateParameter(); p2.ParameterName = "@column"; p2.Value = field.ColumnName;
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);

                exists = Convert.ToInt64(await cmd.ExecuteScalarAsync()) > 0;
            }

            if (!exists)
            {
                var constraintName = $"fk_{tableName}_{field.ColumnName}";
                using var alter = conn.CreateCommand();
                alter.CommandText = $"""
                    ALTER TABLE "{tableName}"
                    ADD CONSTRAINT "{constraintName}"
                    FOREIGN KEY ("{field.ColumnName}")
                    REFERENCES "{field.RelationshipSettings!.Slug}" (id)
                    ON DELETE RESTRICT;
                    """;
                await alter.ExecuteNonQueryAsync();
            }
        }
        finally
        {
            if (!wasOpen) await conn.CloseAsync();
        }
    }
}