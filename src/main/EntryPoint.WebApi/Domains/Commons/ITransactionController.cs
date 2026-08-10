using System.Threading.Tasks;
using EntryPoint.WebApi.Commons.Controllers;
using EntryPoint.WebApi.Domains.Transactions.Models;
using Microsoft.AspNetCore.Mvc;

namespace EntryPoint.WebApi.Domains.Commons;

public interface ITransactionController : IController
{
}

public interface IRegisterTransactionController : ITransactionController
{
    Task<IActionResult> ExecuteAsync(RegisterTransactionRequest request);
}
