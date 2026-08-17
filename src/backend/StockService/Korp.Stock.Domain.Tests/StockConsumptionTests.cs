using Korp.Stock.Domain.Entities;

namespace Korp.Stock.Application.Tests;

public class StockConsumptionTests
{
    [Fact]
    public void Create_WhenOneProductHasInsufficientBallance_ShouldFailWithoutChangingAnyProduct()
    {
        // Arrange
        var productA = Product.Create("P001", "Product 1", 10);
        var productB = Product.Create("P002", "Product 2", 2);

        var items = new[]
        {
                new StockConsumptionItem(productA.Value!, 5),
                new StockConsumptionItem(productB.Value!, 5)
            };

        // Act
        var result = StockConsumption.Create(items);

        // Assert
        Assert.False(result.IsSuccess);

        Assert.Equal(10, productA.Value!.Balance);
        Assert.Equal(2, productB.Value!.Balance);

    }

    [Fact]
    public void Create_WhenAllProductsHaveSufficientBalance_ShouldSucceed()
    {
        // Arrange
        var productA = Product.Create(
            "P001",
            "Product 1",
            10);

        var productB = Product.Create(
            "P002",
            "Product 2",
            20);

        var items = new[]
        {
        new StockConsumptionItem(productA.Value!, 5),
        new StockConsumptionItem(productB.Value!, 10)
    };

        // Act
        var result = StockConsumption.Create(items);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void Create_WhenItemsIsEmpty_ShouldFail()
    {
        // Arrange
        var items = Array.Empty<StockConsumptionItem>();

        // Act
        var result = StockConsumption.Create(items);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_WhenItemQuantityIsInvalid_ShouldFail(int quantity)
    {
        // Arrange
        var product = Product.Create(
            "P001",
            "Product",
            10);

        var items = new[]
        {
        new StockConsumptionItem(product.Value!, quantity)
    };

        // Act
        var result = StockConsumption.Create(items);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Apply_ShouldDecreaseBalanceOfAllProducts()
    {
        // Arrange
        var productA = Product.Create(
            "P001",
            "Product 1",
            10);

        var productB = Product.Create(
            "P002",
            "Product 2",
            20);

        var items = new[]
        {
        new StockConsumptionItem(productA.Value!, 5),
        new StockConsumptionItem(productB.Value!, 10)
    };

        var result = StockConsumption.Create(items);

        Assert.True(result.IsSuccess);

        var consumption = result.Value!;

        // Act
        consumption.Apply();

        // Assert
        Assert.Equal(5, productA.Value!.Balance);
        Assert.Equal(10, productB.Value!.Balance);
    }
}
