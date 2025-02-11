using Core.Commons;
using Infra.PostgreSql.Commons.Context;
using Infra.PostgreSql.Domains.Mappers;
using Infra.PostgreSql.Domains.Models;
using System.Threading.Tasks;
using Core.Domains.Transactions.Gateways;
using Core.Domains.Transactions.Models;

namespace Infra.PostgreSql.Domains.Providers;

public sealed class RegisterTransactionProvider(IUnitOfWork unitOfWork) : IRegisterTransactionGateway
{
    private readonly IUnitOfWork _unitOfWork = ArgumentValidator.CheckNull(unitOfWork, nameof(unitOfWork));

    public async Task<Transaction> ExecuteAsync(Transaction transaction)
    {
        return await _unitOfWork.InvokeAsync(async () =>
            {
                TransactionEntity newTransactionEntity = TransactionMapper.ToEntity(transaction);

                TransactionEntity transactionEntity = await _unitOfWork.Transactions.AddAsync(newTransactionEntity);

                return TransactionMapper.ToModel(transactionEntity);
            }
        );
    }
}
