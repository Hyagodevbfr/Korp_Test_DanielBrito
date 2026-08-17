using Korp.Billing.Domain.Common;

namespace Korp.Billing.Domain.Entities;

public class InvoiceItem
{
    public int Id { get; private set; }
    public int InvoiceId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductCode { get; private set; } = null!;
    public string ProductDescription { get; private set; } = null!;
    public int Quantity { get; private set; }

    private InvoiceItem() { }

    public static Result<InvoiceItem> Create(
        int productId,
        string productCode,
        string productDescription,
        int quantity)
    {
        if (productId <= 0)
            return Result<InvoiceItem>.Failure("Id de produto inválido.");

        if (string.IsNullOrWhiteSpace(productCode))
            return Result<InvoiceItem>.Failure("Código de produto inválido.");

        if (string.IsNullOrWhiteSpace(productDescription))
            return Result<InvoiceItem>.Failure("Descrição de produto inválida.");

        if (quantity <= 0)
            return Result<InvoiceItem>.Failure("Quantidade deve ser maior que zero.");

        var invoiceItem = new InvoiceItem
        {
            ProductId = productId,
            ProductCode = productCode.Trim(),
            ProductDescription = productDescription.Trim(),
            Quantity = quantity
        };

        return Result<InvoiceItem>.Success(invoiceItem);
    }
}
