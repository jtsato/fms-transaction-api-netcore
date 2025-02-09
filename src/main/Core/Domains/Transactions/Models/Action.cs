using Core.Commons;

namespace Core.Domains.Transactions.Models;

public sealed class Action : Enumeration<Action>
{
    public static readonly Action Inserted = new Action(0, "INSERTED");
    public static readonly Action Modified = new Action(1, "MODIFIED");
    public static readonly Action Deleted = new Action(2, "DELETED");

    private Action(int id, string name) : base(id, name)
    {
    }
}
