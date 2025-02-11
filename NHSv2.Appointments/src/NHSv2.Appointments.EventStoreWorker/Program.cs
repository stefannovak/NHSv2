using System.Data.SqlClient;
using NHSv2.Appointments.EventStoreWorker;
using NHSv2.Appointments.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<AppointmentProjections>();
builder.Services.AddEventStore(builder.Configuration.GetValue<string>("EventStore:ConnectionString")!);
builder.Services.AddAppointmentsDbContext(builder.Configuration.GetValue<string>("SqlServer:ConnectionString")!);

var host = builder.Build();

// Seed database
await using (var connection = new SqlConnection(builder.Configuration.GetValue<string>("SqlServer:ConnectionString")!))
{
    await connection.OpenAsync();
    
    var command = new SqlCommand("USE master", connection);
    command.ExecuteNonQuery();
    
    command = new SqlCommand("IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Checkpoints') CREATE TABLE Checkpoints (StreamName NVARCHAR(255), Position BIGINT)", connection);
    command.ExecuteNonQuery();
    
    // Add Appointments table
    command = new SqlCommand("IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Appointments') CREATE TABLE Appointments (Id UNIQUEIDENTIFIER PRIMARY KEY, Test NVARCHAR(255))", connection);
    command.ExecuteNonQuery();
    
    command = new SqlCommand("IF NOT EXISTS (SELECT * FROM Checkpoints WHERE StreamName = 'appointments') INSERT INTO Checkpoints (StreamName, Position) VALUES ('appointments', 0)", connection);
    command.ExecuteNonQuery();
}

host.Run();