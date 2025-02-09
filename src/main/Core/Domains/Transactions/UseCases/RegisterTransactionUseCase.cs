using System.Threading.Tasks;
using Core.Commons;
using Core.Domains.Transactions.Gateways;
using Core.Domains.Transactions.Models;

namespace Core.Domains.Transactions.UseCases;

public class RegisterTransactionUseCase : IRegisterTransactionUseCase
{
    private readonly IRegisterTransactionGateway _registerTransactionGateway;
    
    public RegisterTransactionUseCase(IServiceResolver serviceResolver)
    {
        ArgumentValidator.CheckNull(serviceResolver, nameof(serviceResolver));
        _registerTransactionGateway = serviceResolver.Resolve<IRegisterTransactionGateway>();
    }
    
    public async Task<Transaction> ExecuteAsync(Transaction transaction)
    {
        return await _registerTransactionGateway.ExecuteAsync(transaction);
    }
}
