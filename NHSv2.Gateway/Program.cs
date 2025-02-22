using NHSv2.Gateway;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Services
    .AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri("http://localhost:4317");
            })
            .AddJaegerExporter()
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("NHSv2.Gateway")
                .AddTelemetrySdk()
                .AddEnvironmentVariableDetector())
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddSqlClientInstrumentation()
            .AddSource(ActivitySourceHelper.ActivitySource.Name);
    });

builder.Services
    .AddAuthentication("Keycloak")
    .AddJwtBearer("Keycloak", options =>
    {
        options.Authority = "http://localhost:8080/realms/NHSv2-dev";
        options.TokenValidationParameters.ValidateAudience = false;
        options.RequireHttpsMetadata = false;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.UseOcelot();
app.UseHttpsRedirection();
app.Run();