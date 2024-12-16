using System.Text.Json;
using EventStore.Client;
using NHSv2.Appointments.Domain.Appointments;
using NHSv2.Appointments.Domain.Appointments.Events;

namespace NHSv2.Appointments.EventStoreWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string connectionString = "esdb://localhost:2113?tls=false&tlsVerifyCert=false";
        var settings = EventStoreClientSettings.Create(connectionString);
        var eventStoreClient = new EventStoreClient(settings);

        await using var subscription = eventStoreClient.SubscribeToStream(
            "appointments",
            FromStream.Start,
            // EventAppeared(),
            cancellationToken: stoppingToken);
        
        await foreach (var message in subscription.Messages.WithCancellation(stoppingToken)) {
            switch (message) {
                case StreamMessage.Event(var evnt):
                    await HandleEvent(evnt);
                    break;
            }
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
    }
    
    private async Task HandleAppointmentCreated(ResolvedEvent evnt)
    {
        var appointment = JsonSerializer.Deserialize<Appointment>(evnt.Event.Data.Span);
        Console.WriteLine($"Handling appointment created: {appointment.Id}");
    }
    
    private async Task HandleAppointmentUpdatedEvent(ResolvedEvent evnt)
    {
        var appointment = JsonSerializer.Deserialize<Appointment>(evnt.Event.Data.Span);
        Console.WriteLine($"Handling appointment updated: {appointment.Id}");
    }
}