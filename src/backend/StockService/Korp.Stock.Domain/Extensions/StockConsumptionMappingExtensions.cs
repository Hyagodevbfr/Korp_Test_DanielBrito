using Korp.Stock.Domain.Common;
using Korp.Stock.Domain.Dtos;
using Korp.Stock.Domain.Entities;

namespace Korp.Stock.Domain.Extensions;

public static class StockConsumptionMappingExtensions
{
    /// <summary>
    /// Combina o request recebido pela API (identificadores + quantidades) com os produtos
    /// já carregados do repositório, retornando a entidade de domínio pronta para validação.
    /// </summary>
    public static Result<StockConsumption> ToEntity(
        this ConsumeStockRequest request,
        IReadOnlyDictionary<int, Product> productsById)
    {
        var items = new List<StockConsumptionItem>();
        var notFound = new List<int>();

        foreach (var item in request.Items)
        {
            if (productsById.TryGetValue(item.ProductId, out var product))
                items.Add(new StockConsumptionItem(product, item.Quantity));
            else
                notFound.Add(item.ProductId);
        }

        if (notFound.Count > 0)
            return Result<StockConsumption>.Failure(
                $"Produto(s) não encontrado(s): {string.Join(", ", notFound)}.", ErrorType.NotFound);

        return StockConsumption.Create(items);
    }
}
