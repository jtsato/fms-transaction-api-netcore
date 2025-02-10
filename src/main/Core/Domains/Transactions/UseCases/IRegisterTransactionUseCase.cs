using System.Threading.Tasks;
using Core.Domains.Transactions.Commands;
using Core.Domains.Transactions.Models;

namespace Core.Domains.Transactions.UseCases;

public interface IRegisterTransactionUseCase
{
    Task<Transaction> ExecuteAsync(RegisterTransactionCommand command);    
}
