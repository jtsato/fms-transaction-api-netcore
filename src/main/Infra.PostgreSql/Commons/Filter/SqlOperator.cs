using Core.Commons;

namespace Infra.PostgreSQL.Commons.Filter;

public sealed class SqlOperator : Enumeration<SqlOperator>
{
    public static readonly SqlOperator Equal = new SqlOperator(0, "=");
    public static readonly SqlOperator NotEqual = new SqlOperator(1, "!=");
    public static readonly SqlOperator GreaterThan = new SqlOperator(2, ">");
    public static readonly SqlOperator GreaterThanOrEqual = new SqlOperator(3, ">=");
    public static readonly SqlOperator LessThan = new SqlOperator(4, "<");
    public static readonly SqlOperator LessThanOrEqual = new SqlOperator(5, "<=");
    public static readonly SqlOperator Like = new SqlOperator(6, "LIKE");
    public static readonly SqlOperator In = new SqlOperator(7, "IN");

    private SqlOperator(int id, string name) : base(id, name)
    {
    }
}