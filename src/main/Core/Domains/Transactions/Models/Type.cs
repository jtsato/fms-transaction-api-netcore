using Core.Commons;

namespace Core.Domains.Transactions.Models;

public sealed class Type : Enumeration<Type>
{
    public static readonly Type Debit = new Type(0, "DEBIT");
    public static readonly Type Credit = new Type(1, "CREDIT");

    private Type(int id, string name) : base(id, name)
    {
    }
}
