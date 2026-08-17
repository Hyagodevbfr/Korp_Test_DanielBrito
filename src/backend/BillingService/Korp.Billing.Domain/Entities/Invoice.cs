using Korp.Billing.Domain.Common;
using Korp.Billing.Domain.Enums;

namespace Korp.Billing.Domain.Entities;

public class Invoice
{
    private readonly List<InvoiceItem> _items = new();

    public int Id { get; private set; }
    public int Number { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    private Invoice() { }

    public static Result<Invoice> Create(IEnumerable<InvoiceItem> items)
    {
        var itemList = items?.ToList() ?? [];

        if (itemList.Count == 0)
            return Result<Invoice>.Failure("Não é possível criar uma nota fiscal sem itens.");

        var invoice = new Invoice
        {
            Status = InvoiceStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        invoice._items.AddRange(itemList);

        return Result<Invoice>.Success(invoice);
    }

    public Result Close()
    {
        if (Status == InvoiceStatus.Closed)
            return Result.Failure("A nota fiscal já está fechada e não pode ser processada novamente.", ErrorType.Conflict);

        if (_items.Count == 0)
            return Result.Failure("A nota fiscal não possui itens.");

        Status = InvoiceStatus.Closed;
        ClosedAt = DateTime.UtcNow;

        return Result.Success();
    }
}
