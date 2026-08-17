using Korp.Billing.Domain.Common;
using Korp.Billing.Domain.Dtos;

namespace Korp.Billing.Application.Abstractions;

public interface IStockServiceClient
{
    Task<Result<IReadOnlyDictionary<int, StockProductDto>>> GetProductsAsync(IEnumerable<int> productIds);
    Task<Result> ConsumeAsync(IEnumerable<StockConsumeItem> items);
}
