using Korp.Billing.Application.Abstractions;
using Korp.Billing.Domain.Dtos;
using Korp.Billing.Domain.Extensions;

namespace Korp.Billing.Application.Invoices.GetInvoices;

public interface IGetInvoicesUseCase
{
    Task<IEnumerable<InvoiceDto>> ExecuteAsync();
}

public class GetInvoicesUseCase(IInvoiceRepository repository) : IGetInvoicesUseCase
{
    public async Task<IEnumerable<InvoiceDto>> ExecuteAsync()
    {
        var invoices = await repository.GetAllAsync();

        return invoices.ToDto();
    }
}
