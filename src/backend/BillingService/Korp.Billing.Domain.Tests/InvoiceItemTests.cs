using Korp.Billing.Domain.Entities;

namespace Korp.Billing.Domain.Tests;

public class InvoiceItemTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var result = InvoiceItem.Create(1, "P001", "Test Product", 5);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.ProductId);
        Assert.Equal("P001", result.Value.ProductCode);
        Assert.Equal("Test Product", result.Value.ProductDescription);
        Assert.Equal(5, result.Value.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidProductId_ShouldFail(int productId)
    {
        // Act
        var result = InvoiceItem.Create(productId, "P001", "Test Product", 5);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidProductCode_ShouldFail(string? productCode)
    {
        // Act
        var result = InvoiceItem.Create(1, productCode!, "Test Product", 5);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidProductDescription_ShouldFail(string? productDescription)
    {
        // Act
        var result = InvoiceItem.Create(1, "P001", productDescription!, 5);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_WithInvalidQuantity_ShouldFail(int quantity)
    {
        // Act
        var result = InvoiceItem.Create(1, "P001", "Test Product", quantity);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
