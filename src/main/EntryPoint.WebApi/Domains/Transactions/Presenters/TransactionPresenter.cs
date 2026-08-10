using Core.Commons;
using Core.Domains.Transactions.Models;
using EntryPoint.WebApi.Domains.Transactions.Models;

namespace EntryPoint.WebApi.Domains.Transactions.Presenters;

public static class TransactionPresenter
{
    public static TransactionResponse Of(Transaction transaction)
    {
        ArgumentValidator.CheckNull(transaction, nameof(transaction));

        return new TransactionResponse
        {
            Id = transaction.Id,
            Description = transaction.Description,
            Amount = transaction.Amount,
            Type = transaction.Type.Name.ToUpperInvariant(),
            Status = transaction.Status.Name.ToUpperInvariant(),
            Date = transaction.Date,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}
