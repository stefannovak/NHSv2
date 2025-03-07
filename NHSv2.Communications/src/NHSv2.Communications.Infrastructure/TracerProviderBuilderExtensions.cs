using System.Reflection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace NHSv2.Communications.Infrastructure;

public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddCommonResourceBuilder(this TracerProviderBuilder builder)
    {
        var serviceName = Assembly.GetEntryAssembly()?.GetName().Name;
        
        return builder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName ?? "NHSv2.Communications")
                .AddTelemetrySdk()
                .AddEnvironmentVariableDetector());
    }
}