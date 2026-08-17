using Korp.Stock.Application.Abstractions;
using Korp.Stock.Domain.Common;
using Korp.Stock.Domain.Dtos;
using Korp.Stock.Domain.Extensions;

namespace Korp.Stock.Application.Products.GetProductById;

public interface IGetProductByIdUseCase
{
    Task<Result<ProductDto>> ExecuteAsync(int id);
}

public class GetProductByIdUseCase(IProductRepository repository) : IGetProductByIdUseCase
{
    public async Task<Result<ProductDto>> ExecuteAsync(int id)
    {
        var product = await repository.GetByIdAsync(id);

        if (product is null)
            return Result<ProductDto>.Failure($"Produto '{id}' não encontrado.", ErrorType.NotFound);

        return Result<ProductDto>.Success(product.ToDto());
    }
}
