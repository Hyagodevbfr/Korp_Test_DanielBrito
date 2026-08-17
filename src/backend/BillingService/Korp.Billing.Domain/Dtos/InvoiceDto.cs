using Korp.Billing.Domain.Enums;

namespace Korp.Billing.Domain.Dtos;

public record InvoiceDto(
    int Id,
    int Number,
    InvoiceStatus Status,
    IEnumerable<InvoiceItemDto> Items,
    DateTime CreatedAt,
    DateTime? ClosedAt);
