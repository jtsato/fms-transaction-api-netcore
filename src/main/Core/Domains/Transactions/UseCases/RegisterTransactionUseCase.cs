using System;
using System.Globalization;
using System.Threading.Tasks;
using Core.Commons;
using Core.Domains.Transactions.Commands;
using Core.Domains.Transactions.Gateways;
using Core.Domains.Transactions.Models;
using Type = Core.Domains.Transactions.Models.Type;

namespace Core.Domains.Transactions.UseCases;

public class RegisterTransactionUseCase : IRegisterTransactionUseCase
{
    private readonly IRegisterTransactionGateway _registerTransactionGateway;
    private readonly IGetDateTime _getDateTime;
    
    public RegisterTransactionUseCase(IServiceResolver serviceResolver)
    {
        ArgumentValidator.CheckNull(serviceResolver, nameof(serviceResolver));
        _registerTransactionGateway = serviceResolver.Resolve<IRegisterTransactionGateway>();
        _getDateTime = serviceResolver.Resolve<IGetDateTime>();
    }
    
    public async Task<Transaction> ExecuteAsync(RegisterTransactionCommand command)
    {
        Transaction transaction = new Transaction
        {
            Description = command.Description,
            Amount = decimal.Parse(command.Amount, CultureInfo.InvariantCulture),
            Type = Type.GetByName(command.Type).GetValue(),
            Status = Status.Active,
            Date = DateTime.Parse(command.Date, CultureInfo.InvariantCulture),
            CreatedAt = _getDateTime.Now(),
            UpdatedAt = _getDateTime.Now()
        };

        return await _registerTransactionGateway.ExecuteAsync(transaction);
    }
}
