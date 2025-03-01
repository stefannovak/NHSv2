using EventStore.Client;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NHSv2.Payments.Application.Helpers;
using NHSv2.Payments.Application.Repositories;
using NHSv2.Payments.Application.Services.Contracts;
using NHSv2.Payments.Infrastructure.Data;
using NHSv2.Payments.Infrastructure.EventStore;
using NHSv2.Payments.Infrastructure.Persistence;
using OpenTelemetry.Trace;

namespace NHSv2.Payments.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("payments");
                    h.Password("payments");
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
        
        services.AddTelemetry();
        return services;
    }
    
    public static IServiceCollection AddEventStore(this IServiceCollection serviceCollection, string connectionString)
    {
        var settings = EventStoreClientSettings.Create(connectionString);
        var eventStoreClient = new EventStoreClient(settings);
        serviceCollection.AddSingleton(eventStoreClient);
        serviceCollection.AddSingleton<IEventStoreService, EventStoreService>();
        return serviceCollection;
    }
    
    public static IServiceCollection AddPaymentsDbContextForEventStore(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TransactionsDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        }, ServiceLifetime.Singleton);
        
        services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
        // services.AddSingleton<IEventStoreCheckpointRepository, EventStoreCheckpointRepository>();
        return services;
    }
    
    public static IServiceCollection AddPaymentsDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TransactionsDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        }, ServiceLifetime.Singleton); // TODO: - Figure out these lifetimes
        
        services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
        // services.AddSingleton<IEventStoreCheckpointRepository, EventStoreCheckpointRepository>();
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