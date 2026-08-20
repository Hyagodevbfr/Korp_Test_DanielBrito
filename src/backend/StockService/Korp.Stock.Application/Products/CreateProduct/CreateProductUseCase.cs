using Korp.Stock.Application.Abstractions;
using Korp.Stock.Domain.Common;
using Korp.Stock.Domain.Dtos;
using Korp.Stock.Domain.Extensions;

namespace Korp.Stock.Application.Products.CreateProduct;

public interface ICreateProductUseCase
{
    Task<Result<ProductDto>> ExecuteAsync(CreateProductRequest request);
}

public class CreateProductUseCase(IProductRepository repository) : ICreateProductUseCase
{
    public async Task<Result<ProductDto>> ExecuteAsync(CreateProductRequest request)
    {
        if (await repository.ExistsByCodeAsync(request.Code))
            return Result<ProductDto>.Failure($"Já existe um produto com o código '{request.Code}'.", ErrorType.Conflict);

        var result = request.ToEntity();

        if (result.IsFailure)
            return Result<ProductDto>.Failure(result.Error!, result.ErrorType);

        var product = result.Value!;

        await repository.AddAsync(product);

        var saveResult = await repository.SaveChangesAsync();

        if (saveResult.IsFailure)
            return Result<ProductDto>.Failure(saveResult.Error!, saveResult.ErrorType);

        return Result<ProductDto>.Success(product.ToDto());
    }
}
