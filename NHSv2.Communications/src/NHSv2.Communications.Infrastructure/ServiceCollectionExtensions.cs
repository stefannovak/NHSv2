using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using NHSv2.Communications.Application.Consumers;
using NHSv2.Communications.Application.Helpers;
using NHSv2.Communications.Application.Services.Contracts;
using NHSv2.Communications.Infrastructure.Services;
using OpenTelemetry.Trace;

namespace NHSv2.Communications.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<AppointmentsConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {

                cfg.Host("localhost", "/", h =>
                {
                    h.Username("communications");
                    h.Password("communications");
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddTelemetry();
        return services;
    }
    
    private static IServiceCollection AddTelemetry(this IServiceCollection services)
    {
        services
            .AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddOtlpExporter(x =>
                    {
                        x.Endpoint = new Uri("http://localhost:4317");
                    })
                    .AddJaegerExporter()
                    .AddCommonResourceBuilder()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddSource(ActivitySourceHelper.ActivitySource.Name);
            });
        
        return services;
    }
}