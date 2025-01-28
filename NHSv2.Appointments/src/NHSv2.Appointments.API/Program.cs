using System.Reflection;
using Microsoft.OpenApi.Models;
using NHSv2.Appointments.Application;
using NHSv2.Appointments.Infrastructure;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // options.ExampleFilters();
    options.SwaggerDoc("v1", new OpenApiInfo{ Title = "NHSv2.Appointments.API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Specify the authorization token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    });

    OpenApiSecurityScheme securityScheme = new()
    {
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme,
        },
        In = ParameterLocation.Header,
        Name = "Bearer",
        Scheme = "Bearer",
    };

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() },
    });
    
    options.OperationFilter<NHSv2.Appointments.SwaggerExtensions.SecurityRequirementsOperationFilter>();
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services
    .AddAuthentication("Keycloak")
    .AddJwtBearer("Keycloak", options =>
    {
        options.Authority = "http://localhost:8080/realms/NHSv2-dev";
        options.TokenValidationParameters.ValidateAudience = false;
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("NHSv2.Appointments.Application")));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Patient", policy =>
    {
        policy.RequireRole("patient");
    });
    
    options.AddPolicy("Doctor", policy =>
    {
        policy.RequireRole("doctor");
    });
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// TODO: - API doesn't need the EventStore, but Application does, does API need to provide a connection string?
builder.Services.AddEventStore(builder.Configuration.GetValue<string>("EventStore:ConnectionString")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();