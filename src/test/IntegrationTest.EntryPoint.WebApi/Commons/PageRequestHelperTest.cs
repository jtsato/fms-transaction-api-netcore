using System.Linq;
using Core.Commons.Paging;
using EntryPoint.WebApi.Commons;
using Xunit;

namespace IntegrationTest.EntryPoint.WebApi.Commons;

[Collection("WebApi Collection [NoContext]")]
public sealed class PageRequestHelperTest
{
    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "Successful to create an PageRequest with page number page size and sort")]
    public void SuccessfulToCreateAnPageRequestWithPageNumberPageSizeAndSort()
    {
        // Arrange
        // Act
        PageRequest pageRequest = PageRequestHelper.Of("3", "9", " name:asc , id:desc ");

        // Assert
        Assert.NotNull(pageRequest);
        Assert.Equal(3, pageRequest.PageNumber);
        Assert.Equal(9, pageRequest.PageSize);
        Assert.NotNull(pageRequest.Sort);
        Assert.NotEmpty(pageRequest.Sort.GetOrders());

        Order[] orders = pageRequest.Sort.GetOrders().ToArray();
        Assert.Equal(2, orders.Length);

        Order order1 = orders[0];
        Assert.NotNull(order1);
        Assert.Equal("name", order1.Property);
        Assert.Equal(Direction.Asc, order1.Direction);

        Order order2 = orders[1];
        Assert.NotNull(order2);
        Assert.Equal("id", order2.Property);
        Assert.Equal(Direction.Desc, order2.Direction);
    }

    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "Successful to create an PageRequest with page number page size")]
    public void SuccessfulToCreateAnPageRequestWithPageNumberPageSize()
    {
        // Arrange
        // Act
        PageRequest pageRequest = PageRequestHelper.Of("3", "9", string.Empty);

        // Assert
        Assert.NotNull(pageRequest);
        Assert.Equal(3, pageRequest.PageNumber);
        Assert.Equal(9, pageRequest.PageSize);
        Assert.NotNull(pageRequest.Sort);
        Assert.Empty(pageRequest.Sort.GetOrders());
    }

    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "Successful to create an PageRequest with default values when parameters are invalid")]
    public void SuccessfulToCreateAnPageRequestWithDefaultValuesWhenParametersAreInvalid()
    {
        // Arrange
        // Act
        PageRequest pageRequest = PageRequestHelper.Of("X", "Y", string.Empty);

        // Assert
        Assert.NotNull(pageRequest);
        Assert.Equal(0, pageRequest.PageNumber);
        Assert.Equal(10, pageRequest.PageSize);
        Assert.NotNull(pageRequest.Sort);
        Assert.Empty(pageRequest.Sort.GetOrders());
    }
    
    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "Successful to create an PageRequest with default values when parameters are null")]
    public void SuccessfulToCreateAnPageRequestWithDefaultValuesWhenParametersAreNull()
    {
        // Arrange
        // Act
        PageRequest pageRequest = PageRequestHelper.Of(null, null, null);

        // Assert
        Assert.NotNull(pageRequest);
        Assert.Equal(0, pageRequest.PageNumber);
        Assert.Equal(10, pageRequest.PageSize);
        Assert.NotNull(pageRequest.Sort);
        Assert.Empty(pageRequest.Sort.GetOrders());
    }
    
    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "Successful to create an PageRequest with default values when parameters are empty")]
    public void SuccessfulToCreateAnPageRequestWithDefaultValuesWhenParametersAreEmpty()
    {
        // Arrange
        // Act
        PageRequest pageRequest = PageRequestHelper.Of(string.Empty, string.Empty, string.Empty);

        // Assert
        Assert.NotNull(pageRequest);
        Assert.Equal(0, pageRequest.PageNumber);
        Assert.Equal(10, pageRequest.PageSize);
        Assert.NotNull(pageRequest.Sort);
        Assert.Empty(pageRequest.Sort.GetOrders());
    }
    
    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "Successful to create an PageRequest with default values when parameters are blank")]
    public void SuccessfulToCreateAnPageRequestWithDefaultValuesWhenParametersAreBlank()
    {
        // Arrange
        // Act
        PageRequest pageRequest = PageRequestHelper.Of(" ", " ", " ");

        // Assert
        Assert.NotNull(pageRequest);
        Assert.Equal(0, pageRequest.PageNumber);
        Assert.Equal(10, pageRequest.PageSize);
        Assert.NotNull(pageRequest.Sort);
        Assert.Empty(pageRequest.Sort.GetOrders());
    }
    
    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "Successful to create an PageRequest with default values when parameters are invalid and sort is invalid")]
    public void SuccessfulToCreateAnPageRequestWithDefaultValuesWhenParametersAreInvalidAndSortIsInvalid()
    {
        // Arrange
        // Act
        PageRequest pageRequest = PageRequestHelper.Of("0", "0", " : ");

        // Assert
        Assert.NotNull(pageRequest);
        Assert.Equal(0, pageRequest.PageNumber);
        Assert.Equal(10, pageRequest.PageSize);
        Assert.NotNull(pageRequest.Sort);
        Assert.Empty(pageRequest.Sort.GetOrders());
    }
}