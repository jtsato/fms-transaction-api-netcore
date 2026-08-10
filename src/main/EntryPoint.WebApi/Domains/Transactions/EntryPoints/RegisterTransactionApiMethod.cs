using System.Threading.Tasks;
using Core.Commons;
using EntryPoint.WebApi.Commons;
using EntryPoint.WebApi.Commons.Models;
using EntryPoint.WebApi.Domains.Commons;
using EntryPoint.WebApi.Domains.Transactions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EntryPoint.WebApi.Domains.Transactions.EntryPoints;

[ApiController]
[Route("v1/transactions")]
[ApiExplorerSettings(GroupName = "Transactions")]
[Consumes("application/json")]
[Produces("application/json")]
public sealed class RegisterTransactionApiMethod(IRegisterTransactionController controller) : IApiMethod
{
    private readonly IRegisterTransactionController _controller = ArgumentValidator.CheckNull(controller, nameof(controller));

    [SwaggerOperation(
        OperationId = nameof(RegisterTransaction),
        Tags = ["Transactions"],
        Summary = "Register a transaction.",
        Description = "Registers a debit or credit transaction and returns its persisted representation."
    )]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseStatus), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseStatus), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public Task<IActionResult> RegisterTransaction([FromBody] RegisterTransactionRequest request)
    {
        return _controller.ExecuteAsync(request);
    }
}
