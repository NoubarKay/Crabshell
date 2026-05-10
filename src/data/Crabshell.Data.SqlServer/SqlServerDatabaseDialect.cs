using Crabshell.Core.Attributes;
using Crabshell.Core.Registry;
using Crabshell.Core.Schema;

namespace Crabshell.Data.SqlServer;

public class SqlServerDatabaseDialect : IDatabaseDialect
{
    public string UuidType => "uniqueidentifier";
    public string TimestampType => "datetimeoffset";
    public string BoolType => "bit";
    public string NewUuid() => "NEWID()";
    public string Now() => "GETUTCDATE()";

    public string AddColumnIfNotExists(string table, string columnDdl)
    {
        return $"""
                IF NOT EXISTS (
                    SELECT 1 FROM information_schema.columns
                    WHERE table_name = '{table}' AND column_name = '{ExtractColumnName(columnDdl)}'
                )
                ALTER TABLE [{table}] ADD {columnDdl};
                """;
    }

    public string GetExistingColumnsQuery(string tableName) =>
        $"SELECT column_name FROM information_schema.columns WHERE table_name = '{tableName}'";
    public int ColumnNameResultIndex => 0;

    public string GetColumnType(FieldMeta field)
    {
        var clrType = Nullable.GetUnderlyingType(field.ClrType) ?? field.ClrType;

        return field.FieldType switch
        {
            FieldType.Text         => field.TextSettings?.MaxLength == -1 ? "nvarchar(max)" : $"nvarchar({field.TextSettings?.MaxLength ?? 255})",
            FieldType.RichText     => "nvarchar(max)",
            FieldType.Bool         => "bit",
            FieldType.Relationship => "uniqueidentifier",
            FieldType.Select       => clrType.IsEnum ? "int" : "nvarchar(255)",
            FieldType.DateTime     => field.DateTimeSettings?.TimeOnly == true ? "time"
                                   : field.DateTimeSettings?.HasTime == false  ? "date"
                                   : "datetimeoffset",
            FieldType.Number       => clrType == typeof(int)     ? "int"
                                   : clrType == typeof(long)    ? "bigint"
                                   : clrType == typeof(decimal) ? $"numeric(18,{field.NumberSettings?.Decimals ?? 2})"
                                   : "float",
            _                      => "nvarchar(max)"
        };
    }

    // Extracts the column name from a DDL fragment like "\"col_name\" nvarchar(255) NOT NULL"
    private static string ExtractColumnName(string columnDdl)
    {
        var trimmed = columnDdl.TrimStart('"', '[');
        var end = trimmed.IndexOfAny(['"', ']', ' ']);
        return end > 0 ? trimmed[..end] : trimmed;
    }
}
