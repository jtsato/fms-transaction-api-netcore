using System;
using Swashbuckle.AspNetCore.Annotations;

namespace EntryPoint.WebApi.Domains.Transactions.Models;

public sealed class TransactionResponse
{
    [SwaggerSchema(Description = "Sequential transaction identifier.")]
    public long Id { get; init; }

    [SwaggerSchema(Description = "Human-readable transaction description.")]
    public string Description { get; init; }

    [SwaggerSchema(Description = "Transaction amount.")]
    public decimal Amount { get; init; }

    [SwaggerSchema(Description = "Transaction type.")]
    public string Type { get; init; }

    [SwaggerSchema(Description = "Transaction status.")]
    public string Status { get; init; }

    [SwaggerSchema(Description = "Transaction date.")]
    public DateTime Date { get; init; }

    [SwaggerSchema(Description = "Creation timestamp.")]
    public DateTime CreatedAt { get; init; }

    [SwaggerSchema(Description = "Last update timestamp.")]
    public DateTime UpdatedAt { get; init; }
}
