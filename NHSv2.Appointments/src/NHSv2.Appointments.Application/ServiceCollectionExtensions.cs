using Microsoft.Extensions.DependencyInjection;

namespace NHSv2.Appointments.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services;
    }
}