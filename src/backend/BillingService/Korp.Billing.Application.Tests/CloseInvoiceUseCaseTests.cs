using Korp.Billing.Application.Abstractions;
using Korp.Billing.Application.Invoices.CloseInvoice;
using Korp.Billing.Domain.Common;
using Korp.Billing.Domain.Dtos;
using Korp.Billing.Domain.Entities;
using Korp.Billing.Domain.Enums;
using Moq;

namespace Korp.Billing.Application.Tests;

public class CloseInvoiceUseCaseTests
{
    private static Invoice OpenInvoice(int productId = 1, int quantity = 5)
    {
        var item = InvoiceItem.Create(productId, $"P00{productId}", $"Product {productId}", quantity).Value!;

        return Invoice.Create([item]).Value!;
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvoiceDoesNotExist_ShouldFailWithNotFoundWithoutCallingStockService()
    {
        // Arrange
        var repositoryMock = new Mock<IInvoiceRepository>();
        repositoryMock.Setup(repository => repository.GetByIdAsync(1)).ReturnsAsync((Invoice?)null);

        var stockClientMock = new Mock<IStockServiceClient>();

        var useCase = new CloseInvoiceUseCase(repositoryMock.Object, stockClientMock.Object);

        // Act
        var result = await useCase.ExecuteAsync(1);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        stockClientMock.Verify(client => client.ConsumeAsync(It.IsAny<IEnumerable<StockConsumeItem>>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvoiceIsAlreadyClosed_ShouldFailWithConflictWithoutCallingStockService()
    {
        // Arrange: a checagem de "já fechada" precisa impedir a chamada ao Stock Service —
        // senão uma segunda tentativa de fechamento duplicaria a baixa de estoque antes de
        // falhar aqui.
        var invoice = OpenInvoice();
        invoice.Close();

        var repositoryMock = new Mock<IInvoiceRepository>();
        repositoryMock.Setup(repository => repository.GetByIdAsync(1)).ReturnsAsync(invoice);

        var stockClientMock = new Mock<IStockServiceClient>();

        var useCase = new CloseInvoiceUseCase(repositoryMock.Object, stockClientMock.Object);

        // Act
        var result = await useCase.ExecuteAsync(1);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);

        stockClientMock.Verify(client => client.ConsumeAsync(It.IsAny<IEnumerable<StockConsumeItem>>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStockServiceConfirmsConsumption_ShouldCloseInvoiceAndSave()
    {
        // Arrange
        var invoice = OpenInvoice();

        var repositoryMock = new Mock<IInvoiceRepository>();
        repositoryMock.Setup(repository => repository.GetByIdAsync(1)).ReturnsAsync(invoice);

        var stockClientMock = new Mock<IStockServiceClient>();
        stockClientMock
            .Setup(client => client.ConsumeAsync(It.IsAny<IEnumerable<StockConsumeItem>>()))
            .ReturnsAsync(Result.Success());

        var useCase = new CloseInvoiceUseCase(repositoryMock.Object, stockClientMock.Object);

        // Act
        var result = await useCase.ExecuteAsync(1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(InvoiceStatus.Closed, invoice.Status);
        Assert.NotNull(invoice.ClosedAt);

        repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStockServiceFails_ShouldKeepInvoiceOpenAndNotSave()
    {
        // Arrange: simula o Stock Service indisponível/timeout (docs/07-falhas-e-resiliencia)
        var invoice = OpenInvoice();

        var repositoryMock = new Mock<IInvoiceRepository>();
        repositoryMock.Setup(repository => repository.GetByIdAsync(1)).ReturnsAsync(invoice);

        var stockClientMock = new Mock<IStockServiceClient>();
        stockClientMock
            .Setup(client => client.ConsumeAsync(It.IsAny<IEnumerable<StockConsumeItem>>()))
            .ReturnsAsync(Result.Failure(
                "Não foi possível se comunicar com o serviço de estoque.", ErrorType.Unavailable));

        var useCase = new CloseInvoiceUseCase(repositoryMock.Object, stockClientMock.Object);

        // Act
        var result = await useCase.ExecuteAsync(1);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unavailable, result.ErrorType);

        // A nota tem que permanecer Aberta — é a regra mais importante do fechamento.
        Assert.Equal(InvoiceStatus.Open, invoice.Status);
        Assert.Null(invoice.ClosedAt);

        repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }
}
