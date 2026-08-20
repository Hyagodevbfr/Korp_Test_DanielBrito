using Korp.Stock.Application.Abstractions;
using Korp.Stock.Application.Products.CreateProduct;
using Korp.Stock.Domain.Common;
using Korp.Stock.Domain.Dtos;
using Korp.Stock.Domain.Entities;
using Moq;

namespace Korp.Stock.Application.Tests
{
    public class ApplicationTests
    {
        [Fact]
        public async Task ExecuteAsync_WithNewCode_ShouldCreateProduct()
        {
            // Arrange
            var request = new CreateProductRequest("P001", "Test Product", 10);

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock
                .Setup(repository => repository.ExistsByCodeAsync(request.Code, null))
                .ReturnsAsync(false);
            productRepositoryMock
                .Setup(repository => repository.SaveChangesAsync())
                .ReturnsAsync(Result.Success());

            var useCase = new CreateProductUseCase(productRepositoryMock.Object);

            // Act
            var result = await useCase.ExecuteAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(request.Code, result.Value!.Code);

            productRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Product>()), Times.Once);
            productRepositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WithExistingCode_ShouldFailWithConflict()
        {
            // Arrange
            var request = new CreateProductRequest("P001", "Test Product", 10);

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock
                .Setup(repository => repository.ExistsByCodeAsync(request.Code, null))
                .ReturnsAsync(true);

            var useCase = new CreateProductUseCase(productRepositoryMock.Object);

            // Act
            var result = await useCase.ExecuteAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.Conflict, result.ErrorType);

            productRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Product>()), Times.Never);
            productRepositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidData_ShouldFailWithoutPersisting()
        {
            // Arrange
            var request = new CreateProductRequest("P001", "Test Product", -5);

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock
                .Setup(repository => repository.ExistsByCodeAsync(request.Code, null))
                .ReturnsAsync(false);

            var useCase = new CreateProductUseCase(productRepositoryMock.Object);

            // Act
            var result = await useCase.ExecuteAsync(request);

            // Assert
            Assert.False(result.IsSuccess);

            productRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Product>()), Times.Never);
            productRepositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Never);
        }
    }
}
