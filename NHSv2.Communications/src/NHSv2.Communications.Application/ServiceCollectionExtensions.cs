using Microsoft.Extensions.DependencyInjection;
using NHSv2.Communications.Application.Services;
using NHSv2.Communications.Application.Services.Contracts;

namespace NHSv2.Communications.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}