using Crabshell.Core.Registry;

namespace Crabshell.Core.Schema;

public interface IDatabaseDialect
{
    string UuidType { get; }
    string TimestampType { get; }
    string BoolType { get; }
    string NewUuid();
    string Now();
    string AddColumnIfNotExists(string table, string columnDdl);
    string GetColumnType(FieldMeta field);
    string GetExistingColumnsQuery(string tableName);
    int ColumnNameResultIndex { get; }

}