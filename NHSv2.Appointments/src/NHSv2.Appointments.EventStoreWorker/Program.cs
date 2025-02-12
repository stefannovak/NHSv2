using NHSv2.Appointments.EventStoreWorker;
using NHSv2.Appointments.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfrastructureServices();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString")!;
});

builder.Services.AddAppointmentsDbContextForEventStore(builder.Configuration.GetValue<string>("SqlServer:ConnectionString")!);
builder.Services.AddEventStore(builder.Configuration.GetValue<string>("EventStore:ConnectionString")!);
builder.Services.AddHostedService<AppointmentProjections>();

var host = builder.Build();
host.Run();