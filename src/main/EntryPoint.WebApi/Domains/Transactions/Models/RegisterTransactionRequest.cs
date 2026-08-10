using Swashbuckle.AspNetCore.Annotations;

namespace EntryPoint.WebApi.Domains.Transactions.Models;

public sealed class RegisterTransactionRequest
{
    [SwaggerSchema(Description = "Human-readable transaction description.")]
    public string Description { get; init; }

    [SwaggerSchema(Description = "Transaction amount represented as a decimal string.")]
    public string Amount { get; init; }

    [SwaggerSchema(Description = "Transaction type: DEBIT or CREDIT.")]
    public string Type { get; init; }

    [SwaggerSchema(Description = "Transaction date in an ISO-compatible format.")]
    public string Date { get; init; }
}
