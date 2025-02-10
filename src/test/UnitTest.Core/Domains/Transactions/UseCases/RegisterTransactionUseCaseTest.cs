using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Core.Commons;
using Core.Domains.Transactions.Commands;
using Core.Domains.Transactions.Gateways;
using Core.Domains.Transactions.Models;
using Core.Domains.Transactions.UseCases;
using Moq;
using UnitTest.Core.Commons;
using Xunit;
using Xunit.Abstractions;
using Type = Core.Domains.Transactions.Models.Type;

namespace UnitTest.Core.Domains.Transactions.UseCases;

[ExcludeFromCodeCoverage]
public sealed class RegisterTransactionUseCaseTest : IDisposable
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly Mock<IRegisterTransactionGateway> _registerTransactionGateway;
    private readonly Mock<IGetDateTime> _getDateTime;
    private readonly IRegisterTransactionUseCase _useCase;
    
    public RegisterTransactionUseCaseTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _registerTransactionGateway = new Mock<IRegisterTransactionGateway>();
        _getDateTime = new Mock<IGetDateTime>();

        ServiceResolverMocker serviceResolverMocker = new ServiceResolverMocker()
            .AddService(_registerTransactionGateway.Object)
            .AddService(_getDateTime.Object);

        _useCase = new RegisterTransactionUseCase(serviceResolverMocker.Object);        
    }
    
    private bool _disposed;

    ~RegisterTransactionUseCaseTest()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        _registerTransactionGateway.VerifyAll();
        _getDateTime.VerifyAll();
        Dispose(true);
        _outputHelper.WriteLine($"{nameof(RegisterTransactionUseCaseTest)} disposed.");
        GC.SuppressFinalize(this);
    }

    [ExcludeFromCodeCoverage]
    private void Dispose(bool disposing)
    {
        if (_disposed || !disposing) return;
        _disposed = true;
    }

    [Trait("Category", "Core Business tests")]
    [Fact(DisplayName = "Successful to register transaction")]
    public async Task SuccessfulToRegisterTransaction()
    {
        // Arrange
        _getDateTime
            .Setup(gateway => gateway.Now())
            .Returns(new DateTime(2021, 4, 23, 10, 0, 1, DateTimeKind.Local));
        
        _registerTransactionGateway
            .Setup(gateway => gateway.ExecuteAsync(
                new Transaction
                {
                    Id = 0,
                    Description = "Blue",
                    Amount = new decimal(1234.56),
                    Type = Type.Credit,
                    Status = Status.Active,
                    Date = new DateTime(2021, 4, 23, 10, 0, 1, DateTimeKind.Local),
                    CreatedAt = new DateTime(2021, 4, 23, 10, 0, 1, DateTimeKind.Local),
                    UpdatedAt = new DateTime(2021, 4, 23, 10, 0, 1, DateTimeKind.Local),
                }))
            .ReturnsAsync(
                new Transaction
                {
                    Id = 1,
                    Description = "Blue",
                    Amount = new decimal(1234.56),
                    Type = Type.Credit,
                    Status = Status.Active,
                    Date = new DateTime(2021, 4, 23, 10, 0, 1, DateTimeKind.Local),
                    CreatedAt = new DateTime(2021, 4, 23, 10, 0, 1, DateTimeKind.Local),
                    UpdatedAt = new DateTime(2021, 4, 23, 10, 0, 1, DateTimeKind.Local),
                }
                );
        
        // Act
        Transaction actual = await _useCase.ExecuteAsync
        (
            new RegisterTransactionCommand("Blue", "1234.56", "Credit", "2021-04-23 10:00:01")
        );

        // Assert
        Assert.NotNull(actual);

        Assert.Equal(1, actual.Id);
        Assert.Equal(new decimal(1234.56), actual.Amount);
        Assert.Equal("Blue", actual.Description);
        Assert.Equal(Type.Credit, actual.Type);
        Assert.Equal(Status.Active, actual.Status);
        Assert.Equal(new DateTime(2021, 4, 23, 10, 0, 1, DateTimeKind.Local), actual.Date);
        Assert.Equal(new DateTime(2021, 4, 23, 10, 0, 1, DateTimeKind.Local), actual.CreatedAt);
        Assert.Equal(new DateTime(2021, 4, 23, 10, 0, 1, DateTimeKind.Local), actual.UpdatedAt);
    }
}