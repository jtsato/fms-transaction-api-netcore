using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Core.Domains.Transactions.Models;

public sealed class TransactionAudit
{
    public long Id { get; init; }
    public Action Action { get; init; }
    public string OldDescription { get; init; }
    public decimal OldAmount { get; init; }
    public Type OldType { get; init; }
    public DateTime ChangeDate { get; init; }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return new StringBuilder()
            .AppendLine($"{nameof(Id)}: {Id}")
            .AppendLine($"{nameof(Action)}: {Action}")
            .AppendLine($"{nameof(OldDescription)}: {OldDescription}")
            .AppendLine($"{nameof(OldAmount)}: {OldAmount}")
            .AppendLine($"{nameof(OldType)}: {OldType}")
            .AppendLine($"{nameof(ChangeDate)}: {ChangeDate}")
            .ToString();
    }
}
