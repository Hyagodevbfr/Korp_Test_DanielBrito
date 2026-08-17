using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Korp.Stock.Infrastructure.Persistence;

public class StockDbContextFactory : IDesignTimeDbContextFactory<StockDbContext>
{
    public StockDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("StockDatabase");

        var optionsBuilder = new DbContextOptionsBuilder<StockDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new StockDbContext(optionsBuilder.Options);
    }
}
