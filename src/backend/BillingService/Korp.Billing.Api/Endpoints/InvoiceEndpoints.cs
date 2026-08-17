using Korp.Billing.Api.Extensions;
using Korp.Billing.Application.Invoices.CloseInvoice;
using Korp.Billing.Application.Invoices.CreateInvoice;
using Korp.Billing.Application.Invoices.GetInvoiceById;
using Korp.Billing.Application.Invoices.GetInvoices;
using Korp.Billing.Domain.Dtos;

namespace Korp.Billing.Api.Endpoints;

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this WebApplication app)
    {
        app.MapPost("/api/invoices",
            async (
                CreateInvoiceRequest request,
                ICreateInvoiceUseCase useCase) =>
            {
                var result = await useCase.ExecuteAsync(request);

                if (result.IsFailure)
                    return result.ToHttpResult();

                return Results.Created($"/api/invoices/{result.Value!.Id}", result.Value);
            });

        app.MapGet("/api/invoices",
            async (IGetInvoicesUseCase useCase) =>
            {
                var invoices = await useCase.ExecuteAsync();

                return Results.Ok(invoices);
            });

        app.MapGet("/api/invoices/{id:int}",
            async (
                int id,
                IGetInvoiceByIdUseCase useCase) =>
            {
                var result = await useCase.ExecuteAsync(id);

                return result.ToHttpResult();
            });

        app.MapPost("/api/invoices/{id:int}/close",
            async (
                int id,
                ICloseInvoiceUseCase useCase) =>
            {
                var result = await useCase.ExecuteAsync(id);

                return result.ToHttpResult();
            });
    }
}
