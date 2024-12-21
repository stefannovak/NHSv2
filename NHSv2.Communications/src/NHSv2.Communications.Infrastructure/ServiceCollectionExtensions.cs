using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using NHSv2.Communications.Application.Consumers;

namespace NHSv2.Communications.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
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
        
        return services;
    }
}