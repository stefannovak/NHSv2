using EventStore.Client;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NHSv2.Appointments.Application.Consumers;
using NHSv2.Appointments.Application.Helpers;
using NHSv2.Appointments.Application.Repositories;
using NHSv2.Appointments.Application.Services.Contracts;
using NHSv2.Appointments.Infrastructure.Data;
using NHSv2.Appointments.Infrastructure.Persistence;
using NHSv2.Appointments.Infrastructure.Services;
using OpenTelemetry.Trace;

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
        services.AddTelemetry();
        return services;
    }
    
    public static IServiceCollection AddEventStore(this IServiceCollection serviceCollection, string connectionString)
    {
        var settings = EventStoreClientSettings.Create(connectionString);
        var eventStoreClient = new EventStoreClient(settings);
        serviceCollection.AddSingleton(eventStoreClient);
        return serviceCollection;
    }
    
    public static IServiceCollection AddAppointmentsDbContextForEventStore(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppointmentsDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        }, ServiceLifetime.Singleton);
        
        services.AddSingleton<IAppointmentsRepository, AppointmentsRepository>();
        services.AddSingleton<IEventStoreCheckpointRepository, EventStoreCheckpointRepository>();
        return services;
    }
    
    public static IServiceCollection AddAppointmentsDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppointmentsDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        }, ServiceLifetime.Singleton); // TODO: - Figure out these lifetimes
        
        services.AddSingleton<IAppointmentsRepository, AppointmentsRepository>();
        services.AddSingleton<IEventStoreCheckpointRepository, EventStoreCheckpointRepository>();
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