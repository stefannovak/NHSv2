using Microsoft.Extensions.DependencyInjection;
using NHSv2.Appointments.Application.Services;
using NHSv2.Appointments.Application.Services.Contracts;

namespace NHSv2.Appointments.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient("Keycloak", client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080/");
        });
        
        services.AddScoped<IKeycloakService, KeycloakService>();
        return services;
    }
}