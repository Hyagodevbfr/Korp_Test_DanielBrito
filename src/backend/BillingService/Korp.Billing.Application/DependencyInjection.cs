using Korp.Billing.Application.Invoices.CloseInvoice;
using Korp.Billing.Application.Invoices.CreateInvoice;
using Korp.Billing.Application.Invoices.GetInvoiceById;
using Korp.Billing.Application.Invoices.GetInvoices;
using Microsoft.Extensions.DependencyInjection;

namespace Korp.Billing.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateInvoiceUseCase, CreateInvoiceUseCase>();
        services.AddScoped<IGetInvoicesUseCase, GetInvoicesUseCase>();
        services.AddScoped<IGetInvoiceByIdUseCase, GetInvoiceByIdUseCase>();
        services.AddScoped<ICloseInvoiceUseCase, CloseInvoiceUseCase>();

        return services;
    }
}
