using Korp.Stock.Application.Abstractions;
using Korp.Stock.Infrastructure.Persistence;
using Korp.Stock.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Korp.Stock.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<StockDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("StockDatabase")));

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
