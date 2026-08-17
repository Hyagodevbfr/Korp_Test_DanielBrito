using Korp.Billing.Application.Abstractions;
using Korp.Billing.Domain.Common;
using Korp.Billing.Domain.Dtos;
using Korp.Billing.Domain.Extensions;

namespace Korp.Billing.Application.Invoices.GetInvoiceById;

public interface IGetInvoiceByIdUseCase
{
    Task<Result<InvoiceDto>> ExecuteAsync(int id);
}

public class GetInvoiceByIdUseCase(IInvoiceRepository repository) : IGetInvoiceByIdUseCase
{
    public async Task<Result<InvoiceDto>> ExecuteAsync(int id)
    {
        var invoice = await repository.GetByIdAsync(id);

        if (invoice is null)
            return Result<InvoiceDto>.Failure($"Nota fiscal '{id}' não encontrada.", ErrorType.NotFound);

        return Result<InvoiceDto>.Success(invoice.ToDto());
    }
}
