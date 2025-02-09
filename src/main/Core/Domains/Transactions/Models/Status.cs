using Core.Commons;

namespace Core.Domains.Transactions.Models;

public sealed class Status : Enumeration<Status>
{
    public static readonly Status Active = new Status(0, "ACTIVE");
    public static readonly Status Deleted = new Status(1, "DELETED");

    private Status(int id, string name) : base(id, name)
    {
    }
}
