using NHSv2.Payments.EventStoreWorker;
using NHSv2.Payments.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfrastructureServices();
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString")!;
// });

builder.Services.AddPaymentsDbContextForEventStore(builder.Configuration.GetValue<string>("SqlServer:ConnectionString")!);
builder.Services.AddEventStore(builder.Configuration.GetValue<string>("EventStore:ConnectionString")!);
builder.Services.AddHostedService<PaymentsProjections>();


var host = builder.Build();
host.Run();