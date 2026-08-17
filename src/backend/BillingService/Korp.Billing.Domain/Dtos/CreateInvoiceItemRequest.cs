namespace Korp.Billing.Domain.Dtos;

public record CreateInvoiceItemRequest(int ProductId, int Quantity);
