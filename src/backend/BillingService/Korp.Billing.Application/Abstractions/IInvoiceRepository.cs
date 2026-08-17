using Korp.Billing.Domain.Entities;

namespace Korp.Billing.Application.Abstractions;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(int id);
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task AddAsync(Invoice invoice);
    Task SaveChangesAsync();
}
