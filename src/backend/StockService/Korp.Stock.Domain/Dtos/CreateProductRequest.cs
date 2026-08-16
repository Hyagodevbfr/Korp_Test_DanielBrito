namespace Korp.Stock.Domain.Dtos;

public record CreateProductRequest(string Code, string Description, int Balance);
