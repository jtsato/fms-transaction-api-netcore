namespace Infra.PostgreSQL.Commons.Filter;

public sealed class SqlTemplate
{
    public string TableName { get; init; }
    public string KeyProperty { get; init; }
    public string Insert { get; init; }
    public string Select { get; init; }
    public string SelectByKeyId { get; init; }
    public string Count { get; init; }
    public string Update { get; init; }
    public string Delete { get; init; }
}