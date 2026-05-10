using Crabshell.Core.Attributes;
using Crabshell.Core.Registry;
using Crabshell.Core.Schema;

namespace Crabshell.Data.Postgres;

public class PostgresDatabaseDialect : IDatabaseDialect
{
    public string UuidType => "uuid";
    public string TimestampType => "timestamptz";
    public string BoolType => "boolean";
    public string NewUuid() => "gen_random_uuid()";
    public string Now() => "now()";

    public string AddColumnIfNotExists(string table, string columnDdl)
    {
        return $"""ALTER TABLE "{table}" ADD COLUMN IF NOT EXISTS {columnDdl};""";
    }

    public string GetExistingColumnsQuery(string tableName) =>
        $"SELECT column_name FROM information_schema.columns WHERE table_name = '{tableName}'";
    public int ColumnNameResultIndex => 0;

    public string GetColumnType(FieldMeta field)
    {
        var clrType = Nullable.GetUnderlyingType(field.ClrType) ?? field.ClrType;

        return field.FieldType switch
        {
            FieldType.Text         => field.TextSettings?.MaxLength == -1 ? "text" : $"varchar({field.TextSettings?.MaxLength ?? 255})",
            FieldType.RichText     => "text",
            FieldType.Bool         => "boolean",
            FieldType.Relationship => "uuid",
            FieldType.Select       => clrType.IsEnum ? "integer" : "varchar(255)",
            FieldType.DateTime     => field.DateTimeSettings?.TimeOnly == true ? "timetz"
                                   : field.DateTimeSettings?.HasTime == false  ? "date"
                                   : "timestamptz",
            FieldType.Number       => clrType == typeof(int)     ? "integer"
                                   : clrType == typeof(long)    ? "bigint"
                                   : clrType == typeof(decimal) ? $"numeric(18,{field.NumberSettings?.Decimals ?? 2})"
                                   : "double precision",
            _                      => "text"
        };
    }
}
