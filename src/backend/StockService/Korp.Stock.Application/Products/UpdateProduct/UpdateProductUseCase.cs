using Korp.Stock.Application.Abstractions;
using Korp.Stock.Domain.Common;
using Korp.Stock.Domain.Dtos;
using Korp.Stock.Domain.Extensions;

namespace Korp.Stock.Application.Products.UpdateProduct;

public interface IUpdateProductUseCase
{
    Task<Result<ProductDto>> ExecuteAsync(int id, UpdateProductRequest request);
}

public class UpdateProductUseCase(IProductRepository repository) : IUpdateProductUseCase
{
    public async Task<Result<ProductDto>> ExecuteAsync(int id, UpdateProductRequest request)
    {
        var product = await repository.GetByIdAsync(id);

        if (product is null)
            return Result<ProductDto>.Failure($"Produto '{id}' não encontrado.", ErrorType.NotFound);

        if (await repository.ExistsByCodeAsync(request.Code, excludingId: id))
            return Result<ProductDto>.Failure($"Já existe um produto com o código '{request.Code}'.", ErrorType.Conflict);

        var result = request.ApplyTo(product);

        if (result.IsFailure)
            return Result<ProductDto>.Failure(result.Error!, result.ErrorType);

        var saveResult = await repository.SaveChangesAsync();
        if (saveResult.IsFailure)
            return Result<ProductDto>.Failure(saveResult.Error!, saveResult.ErrorType);

        return Result<ProductDto>.Success(product.ToDto());
    }
}
