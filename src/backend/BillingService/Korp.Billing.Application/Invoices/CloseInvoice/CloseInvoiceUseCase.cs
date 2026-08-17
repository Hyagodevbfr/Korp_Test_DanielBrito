using Korp.Billing.Application.Abstractions;
using Korp.Billing.Domain.Common;
using Korp.Billing.Domain.Dtos;
using Korp.Billing.Domain.Entities;
using Korp.Billing.Domain.Enums;
using Korp.Billing.Domain.Extensions;

namespace Korp.Billing.Application.Invoices.CloseInvoice;

public interface ICloseInvoiceUseCase
{
    Task<Result<InvoiceDto>> ExecuteAsync(int id);
}

public class CloseInvoiceUseCase(
    IInvoiceRepository repository,
    IStockServiceClient stockServiceClient) : ICloseInvoiceUseCase
{
    public async Task<Result<InvoiceDto>> ExecuteAsync(int id)
    {
        var invoice = await repository.GetByIdAsync(id);

        if (invoice is null)
            return Result<InvoiceDto>.Failure($"Nota fiscal '{id}' não encontrada.", ErrorType.NotFound);

        if (invoice.Status == InvoiceStatus.Closed)
            return Result<InvoiceDto>.Failure(
                "A nota fiscal já está fechada e não pode ser processada novamente.", ErrorType.Conflict);

        var consumeItems = invoice.Items.Select(item => new StockConsumeItem(item.ProductId, item.Quantity));

        var consumeResult = await stockServiceClient.ConsumeAsync(consumeItems);

        if (consumeResult.IsFailure)
            return Result<InvoiceDto>.Failure(consumeResult.Error!, consumeResult.ErrorType);

        var closeResult = invoice.Close();

        if (closeResult.IsFailure)
            return Result<InvoiceDto>.Failure(closeResult.Error!, closeResult.ErrorType);

        await repository.SaveChangesAsync();

        return Result<InvoiceDto>.Success(invoice.ToDto());
    }
}
