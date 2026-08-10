using System.Net;
using System.Threading.Tasks;
using Core.Commons;
using Core.Domains.Transactions.Commands;
using Core.Domains.Transactions.UseCases;
using EntryPoint.WebApi.Commons.Controllers;
using EntryPoint.WebApi.Domains.Commons;
using EntryPoint.WebApi.Domains.Transactions.Models;
using EntryPoint.WebApi.Domains.Transactions.Presenters;
using Microsoft.AspNetCore.Mvc;

namespace EntryPoint.WebApi.Domains.Transactions.EntryPoints;

public sealed class RegisterTransactionController(IRegisterTransactionUseCase useCase) : IRegisterTransactionController
{
    private readonly IRegisterTransactionUseCase _useCase = ArgumentValidator.CheckNull(useCase, nameof(useCase));

    public async Task<IActionResult> ExecuteAsync(RegisterTransactionRequest request)
    {
        ArgumentValidator.CheckNull(request, nameof(request));

        RegisterTransactionCommand command = new RegisterTransactionCommand(
            request.Description,
            request.Amount,
            request.Type,
            request.Date);

        Core.Domains.Transactions.Models.Transaction transaction = await _useCase.ExecuteAsync(command);
        TransactionResponse response = TransactionPresenter.Of(transaction);

        return await ResponseBuilder.BuildResponse(HttpStatusCode.Created, response);
    }
}
