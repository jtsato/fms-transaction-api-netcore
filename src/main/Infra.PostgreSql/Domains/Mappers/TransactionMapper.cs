using Core.Domains.Transactions.Models;
using Infra.PostgreSql.Domains.Models;

namespace Infra.PostgreSql.Domains.Mappers;

public static class TransactionMapper
{
    public static Transaction ToModel(TransactionEntity entity)
    {
        return new Transaction
        {
            Id = entity.Id,
            Description = entity.Description,
            Amount = entity.Amount,
            Type = Type.GetByName(entity.Type).GetValue(),
            Status = Status.GetByName(entity.Status).GetValue(),
            Date = entity.Date.ToLocalTime(),
            CreatedAt = entity.CreatedAt.ToLocalTime(),
            UpdatedAt = entity.UpdatedAt.ToLocalTime()
        };
    }

    public static TransactionEntity ToEntity(Transaction model)
    {
        return new TransactionEntity
        {
            Id = model.Id,
            Description = model.Description,
            Amount = model.Amount,
            Type = model.Type.Name.ToUpperInvariant(),
            Status = model.Status.Name.ToUpperInvariant(),
            Date = model.Date.ToUniversalTime(),
            CreatedAt = model.CreatedAt.ToUniversalTime(),
            UpdatedAt = model.UpdatedAt.ToUniversalTime()
        };
    }
}
