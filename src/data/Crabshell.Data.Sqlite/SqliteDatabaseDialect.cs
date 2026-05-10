using Crabshell.Core.Attributes;
using Crabshell.Core.Registry;
using Crabshell.Core.Schema;

namespace Crabshell.Data.Sqlite;

public class SqliteDatabaseDialect : IDatabaseDialect
{
    public string UuidType => "TEXT";
    public string TimestampType => "TEXT";
    public string BoolType => "INTEGER";
    public string NewUuid() => "(lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-4' || substr(lower(hex(randomblob(2))),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(lower(hex(randomblob(2))),2) || '-' || lower(hex(randomblob(6))))";
    public string Now() => "(datetime('now'))";

    public string AddColumnIfNotExists(string table, string columnDdl)
    {
        // SQLite doesn't support IF NOT EXISTS on ALTER TABLE — caller must check first
        return $"""ALTER TABLE "{table}" ADD COLUMN {columnDdl};""";
    }

    public string GetColumnType(FieldMeta field)
    {
        var clrType = Nullable.GetUnderlyingType(field.ClrType) ?? field.ClrType;

        return field.FieldType switch
        {
            FieldType.Text         => "TEXT",
            FieldType.RichText     => "TEXT",
            FieldType.Bool         => "INTEGER",
            FieldType.Relationship => "TEXT",
            FieldType.Select       => clrType.IsEnum ? "INTEGER" : "TEXT",
            FieldType.DateTime     => "TEXT",
            FieldType.Number       => clrType == typeof(int)     ? "INTEGER"
                                   : clrType == typeof(long)    ? "INTEGER"
                                   : clrType == typeof(decimal) ? "REAL"
                                   : "REAL",
            _                      => "TEXT"
        };
    }
}
