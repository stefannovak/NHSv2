using System.Data.SqlClient;
using System.Text.Json;
using EventStore.Client;
using NHSv2.Appointments.Domain.Appointments;
using NHSv2.Appointments.Domain.Appointments.Events;

namespace NHSv2.Appointments.EventStoreWorker;

public class AppointmentProjections : BackgroundService
{
    private readonly ILogger<AppointmentProjections> _logger;
    private readonly string _connectionString = "Server=localhost,5434;Database=master;User Id=sa;Password=Password123!;";
    private const string STREAM_NAME = "appointments";
    
    public AppointmentProjections(ILogger<AppointmentProjections> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string connectionString = "esdb://localhost:2113?tls=false&tlsVerifyCert=false";
        var settings = EventStoreClientSettings.Create(connectionString);
        var eventStoreClient = new EventStoreClient(settings);

        var checkpoint = await GetCheckpoint();

        await using var subscription = eventStoreClient.SubscribeToStream(
            STREAM_NAME,
            FromStream.After(new StreamPosition(Convert.ToUInt32(checkpoint))),
            cancellationToken: stoppingToken);
        
        await foreach (var message in subscription.Messages.WithCancellation(stoppingToken)) {
            switch (message) {
                case StreamMessage.Event(var evnt):
                    await HandleEvent(evnt);
                    break;
            }
        }
    }

    private async Task<long> GetCheckpoint()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand("SELECT Position FROM Checkpoints WHERE StreamName = @StreamName", connection);
            command.Parameters.AddWithValue("@StreamName", STREAM_NAME);
            var result = await command.ExecuteScalarAsync();
            return result is DBNull ? 0 : (long) result;
        }
    }

    private async Task HandleEvent(ResolvedEvent evnt)
    {
        switch (evnt.Event.EventType)
        {
            case nameof(AppointmentCreatedEvent):
                await HandleAppointmentCreated(evnt);
                break;
            case nameof(AppointmentUpdatedEvent):
                await HandleAppointmentUpdatedEvent(evnt);
                break;
        }
        
        await IncrementCheckpoint();
    }
    
    private async Task HandleAppointmentCreated(ResolvedEvent evnt)
    {
        var appointmentCreatedEvent = JsonSerializer.Deserialize<AppointmentCreatedEvent>(evnt.Event.Data.Span);
        Console.WriteLine($"Handling appointment created: {appointmentCreatedEvent.Appointment.Id}");
        await InsertAppointmentToDatabase(appointmentCreatedEvent.Appointment);
    }
    
    private async Task HandleAppointmentUpdatedEvent(ResolvedEvent evnt)
    {
        var appointmentUpdatedEvent = JsonSerializer.Deserialize<AppointmentUpdatedEvent>(evnt.Event.Data.Span);
        Console.WriteLine($"Handling appointment updated: {appointmentUpdatedEvent.id}");
        await UpdateAppointmentInDatabase(new Appointment(appointmentUpdatedEvent.id, appointmentUpdatedEvent.testUpdate));
    }
    
    private async Task InsertAppointmentToDatabase(Appointment appointment)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand("INSERT INTO Appointments (Id, Test) VALUES (@Id, @Test)", connection);
            command.Parameters.AddWithValue("@Id", appointment.Id);
            command.Parameters.AddWithValue("@Test", appointment.Test);
            await command.ExecuteNonQueryAsync();
        }

        _logger.LogInformation($"Inserted appointment {appointment.Id} to database");
    }

    private async Task UpdateAppointmentInDatabase(Appointment appointment)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand("UPDATE Appointments SET Test = @Test WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", appointment.Id);
            command.Parameters.AddWithValue("@Test", appointment.Test);
            await command.ExecuteNonQueryAsync();
        }

        _logger.LogInformation($"Updated appointment {appointment.Id} in database");
    }

    private async Task IncrementCheckpoint()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand("UPDATE Checkpoints SET Position = Position + 1 WHERE StreamName = @StreamName", connection);
            command.Parameters.AddWithValue("@StreamName", STREAM_NAME);
            await command.ExecuteNonQueryAsync();
        }
    }
}