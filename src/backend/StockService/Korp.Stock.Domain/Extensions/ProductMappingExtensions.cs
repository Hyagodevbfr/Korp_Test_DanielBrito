using Korp.Stock.Domain.Common;
using Korp.Stock.Domain.Dtos;
using Korp.Stock.Domain.Entities;

namespace Korp.Stock.Domain.Extensions;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product)
        => new(product.Id, product.Code, product.Description, product.Balance);

    public static IEnumerable<ProductDto> ToDto(this IEnumerable<Product> products)
        => products.Select(product => product.ToDto());

    public static Result<Product> ToEntity(this CreateProductRequest request)
        => Product.Create(request.Code, request.Description, request.Balance);

    public static Result ApplyTo(this UpdateProductRequest request, Product product)
        => product.Update(request.Code, request.Description);

    public static Result ApplyTo(this AddStockRequest request, Product product)
        => product.AddStock(request.Quantity);
}
