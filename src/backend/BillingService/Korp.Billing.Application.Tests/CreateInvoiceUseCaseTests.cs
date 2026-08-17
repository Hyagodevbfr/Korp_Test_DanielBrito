using Korp.Billing.Application.Abstractions;
using Korp.Billing.Application.Invoices.CreateInvoice;
using Korp.Billing.Domain.Common;
using Korp.Billing.Domain.Dtos;
using Korp.Billing.Domain.Entities;
using Moq;

namespace Korp.Billing.Application.Tests;

public class CreateInvoiceUseCaseTests
{
    private static CreateInvoiceRequest Request(int productId = 1, int quantity = 5)
        => new([new CreateInvoiceItemRequest(productId, quantity)]);

    [Fact]
    public async Task ExecuteAsync_WithKnownProduct_ShouldCreateInvoiceWithSnapshot()
    {
        // Arrange
        var products = new Dictionary<int, StockProductDto>
        {
            [1] = new StockProductDto(1, "P001", "Test Product")
        };

        var stockClientMock = new Mock<IStockServiceClient>();
        stockClientMock
            .Setup(client => client.GetProductsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(Result<IReadOnlyDictionary<int, StockProductDto>>.Success(products));

        var repositoryMock = new Mock<IInvoiceRepository>();

        var useCase = new CreateInvoiceUseCase(repositoryMock.Object, stockClientMock.Object);

        // Act
        var result = await useCase.ExecuteAsync(Request());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal("P001", result.Value.Items.First().ProductCode);

        repositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Invoice>()), Times.Once);
        repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProductIsNotFoundInStockService_ShouldFailWithNotFoundWithoutPersisting()
    {
        // Arrange: dicionário vazio simula produto inexistente no Stock Service
        var stockClientMock = new Mock<IStockServiceClient>();
        stockClientMock
            .Setup(client => client.GetProductsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(Result<IReadOnlyDictionary<int, StockProductDto>>.Success(
                new Dictionary<int, StockProductDto>()));

        var repositoryMock = new Mock<IInvoiceRepository>();

        var useCase = new CreateInvoiceUseCase(repositoryMock.Object, stockClientMock.Object);

        // Act
        var result = await useCase.ExecuteAsync(Request());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);

        repositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Invoice>()), Times.Never);
        repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStockServiceIsUnavailable_ShouldFailWithoutPersisting()
    {
        // Arrange
        var stockClientMock = new Mock<IStockServiceClient>();
        stockClientMock
            .Setup(client => client.GetProductsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(Result<IReadOnlyDictionary<int, StockProductDto>>.Failure(
                "Não foi possível se comunicar com o serviço de estoque.", ErrorType.Unavailable));

        var repositoryMock = new Mock<IInvoiceRepository>();

        var useCase = new CreateInvoiceUseCase(repositoryMock.Object, stockClientMock.Object);

        // Act
        var result = await useCase.ExecuteAsync(Request());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unavailable, result.ErrorType);

        repositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Invoice>()), Times.Never);
        repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }
}
