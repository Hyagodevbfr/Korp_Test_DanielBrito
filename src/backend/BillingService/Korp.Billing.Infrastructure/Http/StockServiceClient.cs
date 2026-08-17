using System.Net;
using System.Net.Http.Json;
using Korp.Billing.Application.Abstractions;
using Korp.Billing.Domain.Common;
using Korp.Billing.Domain.Dtos;

namespace Korp.Billing.Infrastructure.Http;

public class StockServiceClient(HttpClient httpClient) : IStockServiceClient
{
    public async Task<Result<IReadOnlyDictionary<int, StockProductDto>>> GetProductsAsync(IEnumerable<int> productIds)
    {
        var idSet = productIds.ToHashSet();

        HttpResponseMessage response;

        try
        {
            response = await httpClient.GetAsync("/api/products");
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            return Result<IReadOnlyDictionary<int, StockProductDto>>.Failure(
                "Não foi possível se comunicar com o serviço de estoque.", ErrorType.Unavailable);
        }

        if (!response.IsSuccessStatusCode)
            return Result<IReadOnlyDictionary<int, StockProductDto>>.Failure(
                "O serviço de estoque retornou uma falha ao consultar os produtos.", ErrorType.Unavailable);

        var products = await response.Content.ReadFromJsonAsync<IEnumerable<StockProductDto>>() ?? [];

        var productsById = products
            .Where(product => idSet.Contains(product.Id))
            .ToDictionary(product => product.Id);

        return Result<IReadOnlyDictionary<int, StockProductDto>>.Success(productsById);
    }

    public async Task<Result> ConsumeAsync(IEnumerable<StockConsumeItem> items)
    {
        HttpResponseMessage response;

        try
        {
            response = await httpClient.PostAsJsonAsync("/api/stock/consume", new { Items = items });
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            return Result.Failure(
                "Não foi possível se comunicar com o serviço de estoque.", ErrorType.Unavailable);
        }

        if (response.IsSuccessStatusCode)
            return Result.Success();

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();
        var message = problem?.Detail ?? "Falha ao solicitar a baixa de estoque.";

        var errorType = response.StatusCode switch
        {
            HttpStatusCode.NotFound => ErrorType.NotFound,
            HttpStatusCode.Conflict => ErrorType.Conflict,
            HttpStatusCode.BadRequest => ErrorType.Validation,
            _ => ErrorType.Unavailable
        };

        return Result.Failure(message, errorType);
    }
}
