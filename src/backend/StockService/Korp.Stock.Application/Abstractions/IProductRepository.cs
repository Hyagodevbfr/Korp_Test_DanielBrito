using Korp.Stock.Domain.Entities;

namespace Korp.Stock.Application.Abstractions;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<bool> ExistsByCodeAsync(string code, int? excludingId = null);
    Task<IReadOnlyDictionary<int, Product>> GetByIdsAsync(IEnumerable<int> ids);
    Task AddAsync(Product product);
    Task SaveChangesAsync();
}
