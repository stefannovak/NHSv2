using System.Text.Json;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using NHSv2.Appointments.Application.Repositories;
using NHSv2.Appointments.Domain.Appointments;
using NHSv2.Appointments.Domain.Appointments.Events;
using NHSv2.Appointments.Infrastructure.Data;

namespace NHSv2.Appointments.EventStoreWorker;

public class AppointmentProjections : BackgroundService
{
    private readonly ILogger<AppointmentProjections> _logger;
    private readonly EventStoreClient _eventStoreClient;
    private readonly AppointmentsDbContext _context;
    private readonly IAppointmentsRepository _appointmentsRepository;
    private const string STREAM_NAME = "appointments";
    
    public AppointmentProjections(
        ILogger<AppointmentProjections> logger,
        EventStoreClient eventStoreClient,
        AppointmentsDbContext context,
        IAppointmentsRepository appointmentsRepository)
    {
        _logger = logger;
        _eventStoreClient = eventStoreClient;
        _context = context;
        _appointmentsRepository = appointmentsRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var checkpoint = await GetCheckpoint();
        
        await using var subscription = _eventStoreClient.SubscribeToStream(
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
        var checkpoint = await _context.EventStoreCheckpoints.FirstOrDefaultAsync(x => x.StreamName == STREAM_NAME);
        return checkpoint?.Position ?? 0;
    }
    
    private async Task HandleEvent(ResolvedEvent evnt)
    {
        switch (evnt.Event.EventType)
        {
            case nameof(AppointmentCreatedEvent):
                await HandleAppointmentCreated(evnt);
                break;
        }
        
        await IncrementCheckpoint();
    }
    
    private async Task HandleAppointmentCreated(ResolvedEvent evnt)
    {
        var appointmentCreatedEvent = JsonSerializer.Deserialize<AppointmentCreatedEvent>(evnt.Event.Data.Span);
        if (appointmentCreatedEvent == null)
        {
            return;
        }

        await InsertAppointmentToDatabase(appointmentCreatedEvent);
        Console.WriteLine($"Handling appointment created: {appointmentCreatedEvent.AppointmentId}");
    }
    
    private async Task InsertAppointmentToDatabase(AppointmentCreatedEvent createdEvent)
    {
        var appointment = new Appointment(
            createdEvent.DoctorId,
            createdEvent.PatientId,
            createdEvent.AppointmentId,
            createdEvent.AppointmentStart,
            createdEvent.FacilityName,
            createdEvent.CalendarEventId);

        await _appointmentsRepository.InsertAsync(appointment);
        _logger.LogInformation($"Inserted appointment {appointment.Id} to database");
    }
    
    private async Task IncrementCheckpoint()
    {
        var checkpoint = _context.EventStoreCheckpoints.First(x => x.StreamName == STREAM_NAME);
        checkpoint.Position++;
        await _context.SaveChangesAsync();
    }
}