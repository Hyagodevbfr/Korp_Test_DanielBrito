namespace Korp.Billing.Domain.Dtos;

public record InvoiceItemDto(
    int Id,
    int ProductId,
    string ProductCode,
    string ProductDescription,
    int Quantity);
