using Korp.Billing.Application.Abstractions;
using Korp.Billing.Domain.Common;
using Korp.Billing.Domain.Dtos;
using Korp.Billing.Domain.Extensions;

namespace Korp.Billing.Application.Invoices.CreateInvoice;

public interface ICreateInvoiceUseCase
{
    Task<Result<InvoiceDto>> ExecuteAsync(CreateInvoiceRequest request);
}

public class CreateInvoiceUseCase(
    IInvoiceRepository repository,
    IStockServiceClient stockServiceClient) : ICreateInvoiceUseCase
{
    public async Task<Result<InvoiceDto>> ExecuteAsync(CreateInvoiceRequest request)
    {
        var productIds = request.Items.Select(item => item.ProductId).Distinct().ToList();

        var productsResult = await stockServiceClient.GetProductsAsync(productIds);

        if (productsResult.IsFailure)
            return Result<InvoiceDto>.Failure(productsResult.Error!, productsResult.ErrorType);

        var invoiceResult = request.ToEntity(productsResult.Value!);

        if (invoiceResult.IsFailure)
            return Result<InvoiceDto>.Failure(invoiceResult.Error!, invoiceResult.ErrorType);

        var invoice = invoiceResult.Value!;

        await repository.AddAsync(invoice);
        await repository.SaveChangesAsync();

        return Result<InvoiceDto>.Success(invoice.ToDto());
    }
}
