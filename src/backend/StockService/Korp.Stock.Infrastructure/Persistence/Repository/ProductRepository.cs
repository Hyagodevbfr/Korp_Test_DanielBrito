using Korp.Stock.Application.Abstractions;
using Korp.Stock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Korp.Stock.Infrastructure.Persistence.Repository;

public class ProductRepository : IProductRepository
{
    private readonly StockDbContext _dbContext;

    public ProductRepository(StockDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbContext.Products.AsNoTracking().ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbContext.Products.FindAsync(id);
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludingId = null)
    {
        var query = _dbContext.Products.Where(p => p.Code == code);

        if (excludingId.HasValue)
            query = query.Where(p => p.Id != excludingId.Value);

        return await query.AnyAsync();
    }

    public async Task<IReadOnlyDictionary<int, Product>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _dbContext.Products
            .Where(p => ids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);
    }

    public async Task AddAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
