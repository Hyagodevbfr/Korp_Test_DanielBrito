namespace Korp.Stock.Domain.Dtos;

public record ConsumeStockRequest(IEnumerable<ConsumeStockItem> Items);
