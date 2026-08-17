using Korp.Stock.Application.Abstractions;
using Korp.Stock.Domain.Common;
using Korp.Stock.Domain.Dtos;
using Korp.Stock.Domain.Extensions;

namespace Korp.Stock.Application.Products.AddStock;

public interface IAddStockUseCase
{
    Task<Result<ProductDto>> ExecuteAsync(int id, AddStockRequest request);
}

public class AddStockUseCase(IProductRepository repository) : IAddStockUseCase
{
    public async Task<Result<ProductDto>> ExecuteAsync(int id, AddStockRequest request)
    {
        var product = await repository.GetByIdAsync(id);

        if (product is null)
            return Result<ProductDto>.Failure($"Produto '{id}' não encontrado.", ErrorType.NotFound);

        var result = request.ApplyTo(product);

        if (result.IsFailure)
            return Result<ProductDto>.Failure(result.Error!, result.ErrorType);

        await repository.SaveChangesAsync();

        return Result<ProductDto>.Success(product.ToDto());
    }
}
