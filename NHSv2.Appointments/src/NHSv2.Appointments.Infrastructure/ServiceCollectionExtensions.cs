using EventStore.Client;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NHSv2.Appointments.Application.Consumers;
using NHSv2.Appointments.Application.Services.Contracts;
using NHSv2.Appointments.Infrastructure.Data;
using NHSv2.Appointments.Infrastructure.Services;

namespace NHSv2.Appointments.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<AppointmentEmailSentConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("appointments");
                    h.Password("appointments");
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
        
        services.AddTransient<ICalendarService, GoogleCalendarService>();
        
        return services;
    }
    
    public static IServiceCollection AddEventStore(this IServiceCollection serviceCollection, string connectionString)
    {
        var settings = EventStoreClientSettings.Create(connectionString);
        var eventStoreClient = new EventStoreClient(settings);
        serviceCollection.AddSingleton(eventStoreClient);
        return serviceCollection;
    }
    
    public static IServiceCollection AddAppointmentsDbContext(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddDbContext<AppointmentsDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
        
        return serviceCollection;
    }
}