using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Infra.PostgreSql.Commons.Repository;

namespace Infra.PostgreSql.Domains.Models;

[Table("transactions")]
public sealed class TransactionEntity : Entity
{
    [Column("description")]
    public string Description { get; set; }
    
    [Column("amount")]
    public decimal Amount { get; set; }
    
    [Column("type")]
    public string Type { get; set; }
    
    [Column("status")]
    public string Status { get; set; }
    
    [Column("date")]
    public DateTime Date { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

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
