using Korp.Billing.Application.Abstractions;
using Korp.Billing.Infrastructure.Http;
using Korp.Billing.Infrastructure.Persistence;
using Korp.Billing.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Korp.Billing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BillingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("BillingDatabase")));

        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        services.AddHttpClient<IStockServiceClient, StockServiceClient>(client =>
        {
            var baseUrl = configuration["StockService:BaseUrl"]
                ?? throw new InvalidOperationException("Configuração 'StockService:BaseUrl' não encontrada.");

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(configuration.GetValue("StockService:TimeoutSeconds", 5));
        });

        return services;
    }
}
