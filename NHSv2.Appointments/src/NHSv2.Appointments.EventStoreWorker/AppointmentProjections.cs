using System.Text.Json;
using EventStore.Client;
using Microsoft.Extensions.Caching.Distributed;
using NHSv2.Appointments.Application.Helpers.Helpers;
using NHSv2.Appointments.Application.Redis;
using NHSv2.Appointments.Application.Repositories;
using NHSv2.Appointments.Domain.Appointments;
using NHSv2.Appointments.Domain.Appointments.Events;

namespace NHSv2.Appointments.EventStoreWorker;

public class AppointmentProjections : BackgroundService
{
    private readonly ILogger<AppointmentProjections> _logger;
    private readonly EventStoreClient _eventStoreClient;
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IEventStoreCheckpointRepository _checkpointRepository;
    private readonly IDistributedCache _cache;
    private const string StreamName = "appointments";
    
    public AppointmentProjections(
        ILogger<AppointmentProjections> logger,
        EventStoreClient eventStoreClient,
        IAppointmentsRepository appointmentsRepository,
        IEventStoreCheckpointRepository checkpointRepository,
        IDistributedCache cache)
    {
        _logger = logger;
        _eventStoreClient = eventStoreClient;
        _appointmentsRepository = appointmentsRepository;
        _checkpointRepository = checkpointRepository;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var checkpoint = await _checkpointRepository.GetCheckpoint(StreamName);
        
        await using var subscription = _eventStoreClient.SubscribeToStream(
            StreamName,
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
    
    private async Task HandleEvent(ResolvedEvent evnt)
    {
        switch (evnt.Event.EventType)
        {
            case nameof(AppointmentCreatedEvent):
                await HandleAppointmentCreated(evnt);
                break;
        }
        
        await _checkpointRepository.IncrementCheckpoint(StreamName);
    }
    
    private async Task HandleAppointmentCreated(ResolvedEvent evnt)
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
        var appointmentCreatedEvent = JsonSerializer.Deserialize<AppointmentCreatedEvent>(evnt.Event.Data.Span);
        if (appointmentCreatedEvent == null)
        {
            return;
        }

        await InsertAppointmentToDatabase(appointmentCreatedEvent);
        _logger.LogInformation($"Handling appointment created: {appointmentCreatedEvent.AppointmentId}");
        
        var cacheKeyToFlush = CacheKeys.AppointmentsByDoctorAndFacility(appointmentCreatedEvent.FacilityName, appointmentCreatedEvent.DoctorId);
        await _cache.RemoveAsync(cacheKeyToFlush);
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
}