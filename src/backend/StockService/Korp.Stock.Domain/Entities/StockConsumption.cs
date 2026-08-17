using Korp.Stock.Domain.Common;

namespace Korp.Stock.Domain.Entities;

/// <summary>
/// Representa a baixa de estoque de múltiplos produtos como uma única operação consistente:
/// ou o saldo de todos os itens é suficiente e todos são debitados, ou nenhum é alterado.
/// </summary>
public class StockConsumption
{
    private readonly List<StockConsumptionItem> _items;

    private StockConsumption(List<StockConsumptionItem> items)
    {
        _items = items;
    }

    public IReadOnlyCollection<StockConsumptionItem> Items => _items.AsReadOnly();

    public static Result<StockConsumption> Create(IEnumerable<StockConsumptionItem> items)
    {
        var itemList = items.ToList();

        if (itemList.Count == 0)
            return Result<StockConsumption>.Failure("A baixa de estoque deve conter ao menos um produto.");

        var errors = new List<string>();

        foreach (var item in itemList)
        {
            if (item.Quantity <= 0)
            {
                errors.Add($"Quantidade inválida para o produto '{item.Product.Code}'.");
                continue;
            }

            if (item.Product.Balance < item.Quantity)
            {
                errors.Add(
                    $"Saldo insuficiente para o produto '{item.Product.Code}' " +
                    $"(disponível: {item.Product.Balance}, solicitado: {item.Quantity}).");
            }
        }

        if (errors.Count > 0)
            return Result<StockConsumption>.Failure(string.Join(" ", errors));

        return Result<StockConsumption>.Success(new StockConsumption(itemList));
    }

    /// <summary>
    /// Efetiva a baixa nos produtos já validados em <see cref="Create"/>.
    /// Não deve falhar, já que todas as regras foram checadas previamente.
    /// </summary>
    public Result Apply()
    {
        foreach (var item in _items)
        {
            var result = item.Product.RemoveStock(item.Quantity);

            if (result.IsFailure)
                return result;
        }

        return Result.Success();
    }
}
