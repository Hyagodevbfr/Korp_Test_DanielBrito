using Korp.Billing.Application.Abstractions;
using Korp.Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Korp.Billing.Infrastructure.Persistence.Repository;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly BillingDbContext _dbContext;

    public InvoiceRepository(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _dbContext.Invoices
            .Include(invoice => invoice.Items)
            .FirstOrDefaultAsync(invoice => invoice.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _dbContext.Invoices
            .Include(invoice => invoice.Items)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _dbContext.Invoices.AddAsync(invoice);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
