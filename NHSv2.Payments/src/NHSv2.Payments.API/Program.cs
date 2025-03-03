using System.Reflection;
using System.Text.Json.Serialization;
using NHSv2.Payments.Application;
using NHSv2.Payments.Application.Configurations;
using NHSv2.Payments.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();
builder.Services.Configure<StripeConfiguration>(builder.Configuration.GetSection(StripeConfiguration.Stripe));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "NHSv2.Payments.API", Version = "v1" });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("NHSv2.Payments.Application")));
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services
    .AddApplicationServices()
    .AddEventStore(builder.Configuration.GetValue<string>("EventStore:ConnectionString")!);

builder.Services.AddPaymentsDbContext(builder.Configuration.GetValue<string>("SqlServer:ConnectionString")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();