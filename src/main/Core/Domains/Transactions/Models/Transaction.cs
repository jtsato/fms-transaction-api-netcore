using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Core.Domains.Transactions.Models;

public sealed class Transaction
{
    public long Id { get; init; }
    public string Description { get; init; }
    public decimal Amount { get; init; }
    public Type Type { get; init; }
    public Status Status { get; init; }
    public DateTime Date { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    [ExcludeFromCodeCoverage]
    private bool Equals(Transaction other)
    {
        return Id == other.Id
               && Description == other.Description
               && Amount == other.Amount
               && Type == other.Type
               && Status == other.Status
               && Date.Equals(other.Date)
               && CreatedAt.Equals(other.CreatedAt)
               && UpdatedAt.Equals(other.UpdatedAt);
    }

    [ExcludeFromCodeCoverage]
    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is Transaction other && Equals(other);
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        HashCode hashCode = new HashCode();
        hashCode.Add(Id);
        hashCode.Add(Description);
        hashCode.Add(Amount);
        hashCode.Add(Type.Id);
        hashCode.Add(Status.Id);
        hashCode.Add(Date);
        hashCode.Add(CreatedAt);
        hashCode.Add(UpdatedAt);
        return hashCode.ToHashCode();
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return new StringBuilder()
            .AppendLine($"{nameof(Id)}: {Id}")
            .AppendLine($"{nameof(Description)}: {Description}")
            .AppendLine($"{nameof(Amount)}: {Amount}")
            .AppendLine($"{nameof(Type)}: {Type}")
            .AppendLine($"{nameof(Status)}: {Status}")
            .AppendLine($"{nameof(Date)}: {Date}")
            .AppendLine($"{nameof(CreatedAt)}: {CreatedAt}")
            .AppendLine($"{nameof(UpdatedAt)}: {UpdatedAt}")
            .ToString();
    }
}
