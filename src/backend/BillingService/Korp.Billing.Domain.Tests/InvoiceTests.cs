using Korp.Billing.Domain.Common;
using Korp.Billing.Domain.Entities;
using Korp.Billing.Domain.Enums;

namespace Korp.Billing.Domain.Tests;

public class InvoiceTests
{
    private static InvoiceItem CreateItem(int productId = 1, int quantity = 5)
        => InvoiceItem.Create(productId, $"P00{productId}", $"Product {productId}", quantity).Value!;

    [Fact]
    public void Create_WithItems_ShouldSucceedAsOpen()
    {
        // Arrange
        var items = new[] { CreateItem() };

        // Act
        var result = Invoice.Create(items);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(InvoiceStatus.Open, result.Value!.Status);
        Assert.Null(result.Value.ClosedAt);
        Assert.Single(result.Value.Items);
    }

    [Fact]
    public void Create_WithMultipleItems_ShouldKeepAllOfThem()
    {
        // Arrange
        var items = new[] { CreateItem(1), CreateItem(2), CreateItem(3) };

        // Act
        var result = Invoice.Create(items);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value!.Items.Count);
        Assert.Contains(result.Value.Items, item => item.ProductId == 1);
        Assert.Contains(result.Value.Items, item => item.ProductId == 2);
        Assert.Contains(result.Value.Items, item => item.ProductId == 3);
    }

    [Fact]
    public void Create_WithoutItems_ShouldFail()
    {
        // Act
        var result = Invoice.Create([]);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Create_WithNullItems_ShouldFail()
    {
        // Act
        var result = Invoice.Create(null!);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Close_WhenOpen_ShouldSucceedAndSetClosedAt()
    {
        // Arrange
        var invoice = Invoice.Create([CreateItem()]).Value!;

        // Act
        var result = invoice.Close();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(InvoiceStatus.Closed, invoice.Status);
        Assert.NotNull(invoice.ClosedAt);
    }

    [Fact]
    public void Close_WhenAlreadyClosed_ShouldFailWithConflict()
    {
        // Arrange
        var invoice = Invoice.Create([CreateItem()]).Value!;
        invoice.Close();

        // Act
        var result = invoice.Close();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);
    }
}
