using Korp.Stock.Application.Abstractions;
using Korp.Stock.Domain.Dtos;
using Korp.Stock.Domain.Extensions;

namespace Korp.Stock.Application.Products.GetProducts;

public interface IGetProductsUseCase
{
    Task<IEnumerable<ProductDto>> ExecuteAsync();
}

public class GetProductsUseCase(IProductRepository repository) : IGetProductsUseCase
{
    public async Task<IEnumerable<ProductDto>> ExecuteAsync()
    {
        var products = await repository.GetAllAsync();

        return products.ToDto();
    }
}
