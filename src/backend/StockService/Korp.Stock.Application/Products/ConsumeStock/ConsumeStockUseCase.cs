using Korp.Stock.Application.Abstractions;
using Korp.Stock.Domain.Common;
using Korp.Stock.Domain.Dtos;
using Korp.Stock.Domain.Extensions;

namespace Korp.Stock.Application.Products.ConsumeStock;

public interface IConsumeStockUseCase
{
    Task<Result> ExecuteAsync(ConsumeStockRequest request);
}

public class ConsumeStockUseCase(IProductRepository repository) : IConsumeStockUseCase
{
    public async Task<Result> ExecuteAsync(ConsumeStockRequest request)
    {
        var productIds = request.Items.Select(item => item.ProductId).Distinct();
        var productsById = await repository.GetByIdsAsync(productIds);

        var consumptionResult = request.ToEntity(productsById);

        if (consumptionResult.IsFailure)
            return Result.Failure(consumptionResult.Error!, consumptionResult.ErrorType);

        var applyResult = consumptionResult.Value!.Apply();

        if (applyResult.IsFailure)
            return applyResult;

        await repository.SaveChangesAsync();

        return Result.Success();
    }
}
