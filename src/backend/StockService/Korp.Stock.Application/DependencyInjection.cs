using Korp.Stock.Application.Products.AddStock;
using Korp.Stock.Application.Products.ConsumeStock;
using Korp.Stock.Application.Products.CreateProduct;
using Korp.Stock.Application.Products.GetProductById;
using Korp.Stock.Application.Products.GetProducts;
using Korp.Stock.Application.Products.UpdateProduct;
using Microsoft.Extensions.DependencyInjection;

namespace Korp.Stock.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateProductUseCase, CreateProductUseCase>();
        services.AddScoped<IGetProductsUseCase, GetProductsUseCase>();
        services.AddScoped<IGetProductByIdUseCase, GetProductByIdUseCase>();
        services.AddScoped<IUpdateProductUseCase, UpdateProductUseCase>();
        services.AddScoped<IAddStockUseCase, AddStockUseCase>();
        services.AddScoped<IConsumeStockUseCase, ConsumeStockUseCase>();

        return services;
    }

}
