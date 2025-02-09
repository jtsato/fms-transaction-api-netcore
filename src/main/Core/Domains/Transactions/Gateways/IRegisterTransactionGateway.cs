using System.Threading.Tasks;
using Core.Domains.Transactions.Models;

namespace Core.Domains.Transactions.Gateways;

public interface IRegisterTransactionGateway
{
    Task<Transaction> ExecuteAsync(Transaction transaction);
}
