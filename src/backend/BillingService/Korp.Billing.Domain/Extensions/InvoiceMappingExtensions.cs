using Korp.Billing.Domain.Common;
using Korp.Billing.Domain.Dtos;
using Korp.Billing.Domain.Entities;

namespace Korp.Billing.Domain.Extensions;

public static class InvoiceMappingExtensions
{
    public static InvoiceItemDto ToDto(this InvoiceItem item)
        => new(item.Id, item.ProductId, item.ProductCode, item.ProductDescription, item.Quantity);

    public static InvoiceDto ToDto(this Invoice invoice)
        => new(
            invoice.Id,
            invoice.Number,
            invoice.Status,
            invoice.Items.Select(item => item.ToDto()),
            invoice.CreatedAt,
            invoice.ClosedAt);

    public static IEnumerable<InvoiceDto> ToDto(this IEnumerable<Invoice> invoices)
        => invoices.Select(invoice => invoice.ToDto());

    /// <summary>
    /// Combina o request recebido pela API (produtos + quantidades) com os produtos já
    /// consultados no Stock Service, retornando a nota fiscal pronta para persistência.
    /// </summary>
    public static Result<Invoice> ToEntity(
        this CreateInvoiceRequest request,
        IReadOnlyDictionary<int, StockProductDto> productsById)
    {
        var items = new List<InvoiceItem>();
        var notFound = new List<int>();
        var errors = new List<string>();

        foreach (var itemRequest in request.Items)
        {
            if (!productsById.TryGetValue(itemRequest.ProductId, out var product))
            {
                notFound.Add(itemRequest.ProductId);
                continue;
            }

            var itemResult = InvoiceItem.Create(
                product.Id,
                product.Code,
                product.Description,
                itemRequest.Quantity);

            if (itemResult.IsFailure)
                errors.Add(itemResult.Error!);
            else
                items.Add(itemResult.Value!);
        }

        if (notFound.Count > 0)
            return Result<Invoice>.Failure(
                $"Produto(s) não encontrado(s): {string.Join(", ", notFound)}.", ErrorType.NotFound);

        if (errors.Count > 0)
            return Result<Invoice>.Failure(string.Join(" ", errors));

        return Invoice.Create(items);
    }
}
