using Microsoft.Extensions.DependencyInjection;
using NHSv2.Payments.Application.Services;
using NHSv2.Payments.Application.Services.Contracts;

namespace NHSv2.Payments.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IWebhookHandlers, WebhookHandlers>();
        return services;
    }
}