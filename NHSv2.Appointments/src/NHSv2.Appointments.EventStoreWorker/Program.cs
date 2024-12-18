using System.Data.SqlClient;
using NHSv2.Appointments.EventStoreWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<AppointmentProjections>();

var host = builder.Build();

// Seed database
await using (var connection = new SqlConnection("Server=localhost,5434;Database=master;User Id=sa;Password=Password123!;"))
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