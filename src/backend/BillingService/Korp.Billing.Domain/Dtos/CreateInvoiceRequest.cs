namespace Korp.Billing.Domain.Dtos;

public record CreateInvoiceRequest(IEnumerable<CreateInvoiceItemRequest> Items);
