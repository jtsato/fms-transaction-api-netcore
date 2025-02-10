using System.Collections.Generic;
using System.Linq;
using Core.Domains.Transactions.Commands;
using FluentValidation;
using Xunit;

namespace UnitTest.Core.Domains.Transactions.Command;

public class RegisterTransactionCommandTest
{
    [Trait("Category", "Core Business tests")]
    [Fact(DisplayName = "Fail to register transaction with empty parameters")]
    public void FailToRegisterTransactionWithEmptyParameters()
    {
        // Arrange
        // Act
        // Assert
        ValidationException validationException = Assert.Throws<ValidationException>(() =>
            new RegisterTransactionCommand(
                string.Empty,
                string.Empty,
                null,
                null
        ));
        
        List<string> errorMessages = validationException
            .Errors
            .Select(failure => failure.ErrorMessage)
            .ToList();
        
        Assert.NotEmpty(errorMessages);
        Assert.Equal(4, errorMessages.Count);
        
        Assert.Contains("ValidationTransactionDescriptionIsNullOrEmpty", errorMessages);
        Assert.Contains("ValidationTransactionAmountIsNullOrEmpty", errorMessages);
        Assert.Contains("ValidationTransactionTypeIsNullOrEmpty", errorMessages);
        Assert.Contains("ValidationTransactionDateIsNullOrEmpty", errorMessages);
    }
    
    [Trait("Category", "Core Business tests")]
    [Fact(DisplayName = "Fail to register transaction with invalid parameters")]
    public void FailToRegisterTransactionWithInvalidParameters()
    {
        // Arrange
        // Act
        // Assert
        ValidationException validationException = Assert.Throws<ValidationException>(() =>
            new RegisterTransactionCommand(
                "Description",
                "Amount",
                "Type",
                "Date"
        ));
        
        List<string> errorMessages = validationException
            .Errors
            .Select(failure => failure.ErrorMessage)
            .ToList();
        
        Assert.NotEmpty(errorMessages);
        Assert.Equal(3, errorMessages.Count);
        
        Assert.Contains("ValidationTransactionAmountIsNotDecimal", errorMessages);
        Assert.Contains("ValidationTransactionTypeIsInvalid", errorMessages);
        Assert.Contains("ValidationTransactionDateIsInvalid", errorMessages);
    }
}