using Korp.Stock.Domain.Entities;

namespace Korp.Stock.Domain.Tests
{
    public class ProductTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            //Arrange
            var code = "P001";
            var description = "Test Product";
            var ballance = 10;

            //Act
            var result = Product.Create(
                code,
                description,
                ballance
            );

            //Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_WithInvalidCode_ShouldFail(string? code)
        {
            //Arrange
            var description = "Test Product";
            var ballance = 10;

            //Act
            var result = Product.Create(
                code,
                description,
                ballance
            );

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Create_WithNullDescription_ShouldFail()
        {
            //Arrange
            var code = "P001";
            string? description = null;
            var ballance = 10;
            //Act
            var result = Product.Create(
                code,
                description,
                ballance
            );
            // Assert
            Assert.False(result.IsSuccess);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Update_WithInvalidDescription_ShouldFail(string? description)
        {
            // Arrange
            var result = Product.Create("P001", "Old Product", 10);
            var product = result.Value!;

            // Act
            var updateResult = product.Update("P002", description);

            // Assert
            Assert.False(updateResult.IsSuccess);

            Assert.Equal("P001", product.Code);
            Assert.Equal("Old Product", product.Description);
        }

        [Fact]
        public void Create_WithNegativeBallance_ShouldFail()
        {
            //Arrange
            var code = "P001";
            var description = "Test Product";
            var ballance = -5;

            //Act
            var result = Product.Create(
                code,
                description,
                ballance
            );

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            // Arrange
            var result = Product.Create("P001", "Old Product", 10);

            var product = result.Value!;

            // Act
            var updateResult = product.Update("P002", "Updated Product");

            // Assert
            Assert.True(updateResult.IsSuccess);
            Assert.Equal("P002", product.Code);
            Assert.Equal("Updated Product", product.Description);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Update_WithInvalidCode_ShouldFail(string? code)
        {
            // Arrange
            var result = Product.Create("P001", "Old Product", 10);
            var product = result.Value!;

            // Act
            var updateResult = product.Update(code, "Updated Product");

            // Assert
            Assert.False(updateResult.IsSuccess);

            Assert.Equal("P001", product.Code);
            Assert.Equal("Old Product", product.Description);
        }

        [Fact]
        public void AddStock_WithValidQuantity_ShouldSucceed()
        {
            // Arrange
            var result = Product.Create("P001", "Test Product", 10);
            var product = result.Value!;

            // Act
            var addStockResult = product.AddStock(5);

            // Assert
            Assert.True(addStockResult.IsSuccess);
            Assert.Equal(15, product.Balance);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(-10)]
        public void AddStock_WithInvalidQuantity_ShouldFail(int quantity)
        {
            // Arrange
            var result = Product.Create("P001", "Test Product", 10);
            var product = result.Value!;

            // Act
            var addStockResult = product.AddStock(quantity);

            // Assert
            Assert.False(addStockResult.IsSuccess);
        }

        [Fact]
        public void RemoveStock_WithValidQuantity_ShouldSucceed()
        {
            // Arrange
            var result = Product.Create("P001", "Test Product", 10);
            var product = result.Value!;

            // Act
            var removeStockResult = product.RemoveStock(5);

            // Assert
            Assert.True(removeStockResult.IsSuccess);
            Assert.Equal(5, product.Balance);
        }

        [Fact]
        public void RemoveStock_WithQuantityExceedingBalance_ShouldFail()
        {
            // Arrange
            var result = Product.Create("P001", "Test Product", 10);
            var product = result.Value!;

            // Act
            var removeStockResult = product.RemoveStock(15);

            // Assert
            Assert.False(removeStockResult.IsSuccess);
        }
    }
}