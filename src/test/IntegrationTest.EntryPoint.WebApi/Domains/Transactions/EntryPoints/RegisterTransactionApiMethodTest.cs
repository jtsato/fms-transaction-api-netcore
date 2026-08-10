using System;
using System.Net;
using System.Threading.Tasks;
using Core.Domains.Transactions.Models;
using EntryPoint.WebApi.Commons.Models;
using EntryPoint.WebApi.Domains.Commons;
using EntryPoint.WebApi.Domains.Transactions.EntryPoints;
using EntryPoint.WebApi.Domains.Transactions.Models;
using IntegrationTest.EntryPoint.WebApi.Commons;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IntegrationTest.EntryPoint.WebApi.Domains.Transactions.EntryPoints;

[Collection("WebApi Collection Context")]
public sealed class RegisterTransactionApiMethodTest(ApiMethodInvokerHolder invokerHolder)
{
    private readonly ApiMethodInvoker _invoker = invokerHolder.GetApiMethodInvoker();

    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "POST /api/transactions should return 201 when the transaction is registered")]
    public async Task SuccessfulToRegisterTransaction()
    {
        // Arrange
        Mock<IRegisterTransactionController> controller = new Mock<IRegisterTransactionController>(MockBehavior.Strict);
        TransactionResponse response = new TransactionResponse
        {
            Id = 1,
            Description = "Blue",
            Amount = 1234.56m,
            Type = "CREDIT",
            Status = "ACTIVE",
            Date = new DateTime(2021, 4, 23, 10, 0, 1),
            CreatedAt = new DateTime(2021, 4, 23, 10, 0, 1),
            UpdatedAt = new DateTime(2021, 4, 23, 10, 0, 1)
        };

        controller
            .Setup(self => self.ExecuteAsync(It.IsAny<RegisterTransactionRequest>()))
            .ReturnsAsync(new ObjectResult(response) {StatusCode = (int) HttpStatusCode.Created});

        RegisterTransactionApiMethod apiMethod = new RegisterTransactionApiMethod(controller.Object);

        // Act
        ObjectResult result = await _invoker.InvokeAsync(() => apiMethod.RegisterTransaction(new RegisterTransactionRequest
        {
            Description = "Blue",
            Amount = "1234.56",
            Type = "Credit",
            Date = "2021-04-23 10:00:01"
        }));

        // Assert
        Assert.Equal((int) HttpStatusCode.Created, result.StatusCode);
        Assert.Equal(response, result.Value);
        controller.Verify(self => self.ExecuteAsync(It.Is<RegisterTransactionRequest>(request =>
            request.Description == "Blue" &&
            request.Amount == "1234.56" &&
            request.Type == "Credit" &&
            request.Date == "2021-04-23 10:00:01")), Times.Once);
    }

    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "POST /api/transactions should return 400 when the transaction is invalid")]
    public async Task FailToRegisterTransactionWithInvalidRequest()
    {
        // Arrange
        Mock<IRegisterTransactionController> controller = new Mock<IRegisterTransactionController>(MockBehavior.Strict);
        controller
            .Setup(self => self.ExecuteAsync(It.IsAny<RegisterTransactionRequest>()))
            .ThrowsAsync(new FluentValidation.ValidationException(
                new[]
                {
                    new FluentValidation.Results.ValidationFailure("Amount", "ValidationTransactionAmountIsNotDecimal")
                }));

        RegisterTransactionApiMethod apiMethod = new RegisterTransactionApiMethod(controller.Object);

        // Act
        ObjectResult result = await _invoker.InvokeAsync(() => apiMethod.RegisterTransaction(new RegisterTransactionRequest
        {
            Description = "Blue",
            Amount = "invalid",
            Type = "Credit",
            Date = "2021-04-23 10:00:01"
        }), "en-US");

        // Assert
        Assert.Equal((int) HttpStatusCode.BadRequest, result.StatusCode);
        ResponseStatus responseStatus = Assert.IsType<ResponseStatus>(result.Value);
        Assert.Contains(responseStatus.Fields, field => field.Name == "amount");
    }
}
