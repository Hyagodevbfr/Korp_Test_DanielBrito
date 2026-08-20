using Korp.Stock.Application.Abstractions;
using Korp.Stock.Application.Products.ConsumeStock;
using Korp.Stock.Domain.Common;
using Korp.Stock.Domain.Dtos;
using Korp.Stock.Domain.Entities;
using Moq;

namespace Korp.Stock.Application.Tests
{
    public class ConsumeStockUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_WithSufficientBalance_ShouldConsumeAndSaveChanges()
        {
            // Arrange
            var product = Product.Create("P001", "Test Product", 10).Value!;

            var request = new ConsumeStockRequest([new ConsumeStockItem(1, 5)]);

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock
                .Setup(repository => repository.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new Dictionary<int, Product> { [1] = product });
            productRepositoryMock
                .Setup(repository => repository.SaveChangesAsync())
                .ReturnsAsync(Result.Success());

            var useCase = new ConsumeStockUseCase(productRepositoryMock.Object);

            // Act
            var result = await useCase.ExecuteAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(5, product.Balance);

            productRepositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithInsufficientBalance_ShouldFailWithoutCallingSaveChanges()
        {
            // Arrange
            var product = Product.Create("P001", "Test Product", 1).Value!;

            var request = new ConsumeStockRequest([new ConsumeStockItem(1, 5)]);

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock
                .Setup(repository => repository.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new Dictionary<int, Product> { [1] = product });

            var useCase = new ConsumeStockUseCase(productRepositoryMock.Object);

            // Act
            var result = await useCase.ExecuteAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(1, product.Balance);

            productRepositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenSaveChangesDetectsConcurrentUpdate_ShouldFailWithConflict()
        {
            // Arrange
            var product = Product.Create("P001", "Test Product", 1).Value!;

            var request = new ConsumeStockRequest([new ConsumeStockItem(1, 1)]);

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock
                .Setup(repository => repository.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new Dictionary<int, Product> { [1] = product });
            productRepositoryMock
                .Setup(repository => repository.SaveChangesAsync())
                .ReturnsAsync(Result.Failure(
                    "O saldo do produto foi alterado por outra operação. Tente novamente.",
                    ErrorType.Conflict));

            var useCase = new ConsumeStockUseCase(productRepositoryMock.Object);

            // Act
            var result = await useCase.ExecuteAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.Conflict, result.ErrorType);

            productRepositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
        }
    }
}
