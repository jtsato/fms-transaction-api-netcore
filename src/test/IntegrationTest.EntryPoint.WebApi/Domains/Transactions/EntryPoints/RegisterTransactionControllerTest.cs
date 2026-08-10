using System;
using System.Net;
using System.Threading.Tasks;
using Core.Domains.Transactions.Models;
using Core.Domains.Transactions.UseCases;
using EntryPoint.WebApi.Domains.Transactions.EntryPoints;
using EntryPoint.WebApi.Domains.Transactions.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Type = Core.Domains.Transactions.Models.Type;

namespace IntegrationTest.EntryPoint.WebApi.Domains.Transactions.EntryPoints;

public sealed class RegisterTransactionControllerTest
{
    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "Successful to register transaction through the controller")]
    public async Task SuccessfulToRegisterTransactionThroughTheController()
    {
        // Arrange
        Mock<IRegisterTransactionUseCase> useCase = new Mock<IRegisterTransactionUseCase>(MockBehavior.Strict);
        Transaction savedTransaction = new Transaction
        {
            Id = 1,
            Description = "Blue",
            Amount = 1234.56m,
            Type = Type.Credit,
            Status = Status.Active,
            Date = new DateTime(2021, 4, 23, 10, 0, 1),
            CreatedAt = new DateTime(2021, 4, 23, 10, 0, 1),
            UpdatedAt = new DateTime(2021, 4, 23, 10, 0, 1)
        };

        useCase
            .Setup(self => self.ExecuteAsync(It.Is<Core.Domains.Transactions.Commands.RegisterTransactionCommand>(command =>
                command.Description == "Blue" &&
                command.Amount == "1234.56" &&
                command.Type == "Credit" &&
                command.Date == "2021-04-23 10:00:01")))
            .ReturnsAsync(savedTransaction);

        RegisterTransactionController controller = new RegisterTransactionController(useCase.Object);

        // Act
        ObjectResult result = Assert.IsType<ObjectResult>(await controller.ExecuteAsync(new RegisterTransactionRequest
        {
            Description = "Blue",
            Amount = "1234.56",
            Type = "Credit",
            Date = "2021-04-23 10:00:01"
        }));

        // Assert
        Assert.Equal((int) HttpStatusCode.Created, result.StatusCode);
        TransactionResponse response = Assert.IsType<TransactionResponse>(result.Value);
        Assert.Equal(1, response.Id);
        Assert.Equal("Blue", response.Description);
        Assert.Equal(1234.56m, response.Amount);
        Assert.Equal("CREDIT", response.Type);
        Assert.Equal("ACTIVE", response.Status);
        useCase.VerifyAll();
    }
}
