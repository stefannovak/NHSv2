using NHSv2.Communications.Application.Configuration;
using NHSv2.Communications.Infrastructure;
using NHSv2.Communications.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddInfrastructureServices();

var host = builder.Build();
host.Run();