using EventStore.Client;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace NHSv2.Appointments.Infrastructure;

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
                    h.Username("guest");
                    h.Password("guest");
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }
    
    public static IServiceCollection AddEventStore(this IServiceCollection serviceCollection, string connectionString)
    {
        var settings = EventStoreClientSettings.Create(connectionString);
        var eventStoreClient = new EventStoreClient(settings);
        serviceCollection.AddSingleton(eventStoreClient);
        return serviceCollection;
    }
}