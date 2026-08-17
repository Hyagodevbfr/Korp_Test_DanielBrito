using Korp.Stock.Api.Extensions;
using Korp.Stock.Application.Products.AddStock;
using Korp.Stock.Application.Products.ConsumeStock;
using Korp.Stock.Application.Products.CreateProduct;
using Korp.Stock.Application.Products.GetProductById;
using Korp.Stock.Application.Products.GetProducts;
using Korp.Stock.Application.Products.UpdateProduct;
using Korp.Stock.Domain.Dtos;

namespace Korp.Stock.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        app.MapPost("/api/products",
            async (
                CreateProductRequest request,
                ICreateProductUseCase useCase) =>
            {
                var result = await useCase.ExecuteAsync(request);

                if (result.IsFailure)
                    return result.ToHttpResult();

                return Results.Created($"/api/products/{result.Value!.Id}", result.Value);
            });

        app.MapGet("/api/products",
            async (IGetProductsUseCase useCase) =>
            {
                var products = await useCase.ExecuteAsync();

                return Results.Ok(products);
            });

        app.MapGet("/api/products/{id:int}",
            async (
                int id,
                IGetProductByIdUseCase useCase) =>
            {
                var result = await useCase.ExecuteAsync(id);

                return result.ToHttpResult();
            });

        app.MapPut("/api/products/{id:int}",
            async (
                int id,
                UpdateProductRequest request,
                IUpdateProductUseCase useCase) =>
            {
                var result = await useCase.ExecuteAsync(id, request);

                return result.ToHttpResult();
            });

        app.MapPost("/api/products/{id:int}/stock",
            async (
                int id,
                AddStockRequest request,
                IAddStockUseCase useCase) =>
            {
                var result = await useCase.ExecuteAsync(id, request);

                return result.ToHttpResult();
            });

        app.MapPost("/api/stock/consume",
            async (
                ConsumeStockRequest request,
                IConsumeStockUseCase useCase) =>
            {
                var result = await useCase.ExecuteAsync(request);

                return result.ToHttpResult();
            });
    }
}
