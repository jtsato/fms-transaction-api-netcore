using System;
using System.Threading.Tasks;
using Core.Domains.Transactions.Gateways;
using Core.Domains.Transactions.Models;
using IntegrationTest.Infra.PostgreSql.Commons;
using IntegrationTest.Infra.PostgreSql.Domains.Providers;
using Xunit;
using Type = Core.Domains.Transactions.Models.Type;

namespace IntegrationTest.Infra.PostgreSql.Domains;

[Collection("Database collection")]
public sealed class RegisterTransactionProviderTest(Context context, RegisterTransactionProviderTestFixture fixture) : IClassFixture<RegisterTransactionProviderTestFixture>
{
    private readonly IRegisterTransactionGateway _provider = context.ServiceResolver.Resolve<IRegisterTransactionGateway>();

    [Trait("Category", "Infrastructure (DB) Integration tests")]
    [Fact(DisplayName = "Successful to create a transaction")]
    public async Task SuccessfulToCreateATransaction()
    {
        // Arrange
        Transaction transaction = new Transaction
        {
            Description = "Sale of iPhone 11 Smartphone",
            Amount = 1999.99m,
            Type = Type.Credit,
            Status = Status.Active,
            Date = new DateTime(2023, 09, 02, 14, 00, 01, DateTimeKind.Local).ToUniversalTime(),
            CreatedAt = new DateTime(2023, 09, 02, 15, 00, 02, DateTimeKind.Local).ToUniversalTime(),
            UpdatedAt = new DateTime(2023, 09, 02, 16, 00, 03, DateTimeKind.Local).ToUniversalTime(),
        };

        // Act
        Transaction actual = await _provider.ExecuteAsync(transaction);

        // Assert
        Assert.NotNull(actual);
        
        Assert.Equal("Sale of iPhone 11 Smartphone", actual.Description);
        Assert.Equal(1999.99m, actual.Amount);
        Assert.Equal(Type.Credit, actual.Type);
        Assert.Equal(Status.Active, actual.Status);
        Assert.Equal(new DateTime(2023, 09, 02, 14, 00, 01, DateTimeKind.Local).ToUniversalTime(), actual.Date.ToUniversalTime());
        Assert.Equal(new DateTime(2023, 09, 02, 15, 00, 02, DateTimeKind.Local).ToUniversalTime(), actual.CreatedAt.ToUniversalTime());
        Assert.Equal(new DateTime(2023, 09, 02, 16, 00, 03, DateTimeKind.Local).ToUniversalTime(), actual.UpdatedAt.ToUniversalTime());
    }
}
