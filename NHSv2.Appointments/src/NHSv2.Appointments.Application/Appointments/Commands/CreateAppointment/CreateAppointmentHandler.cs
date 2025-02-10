using System.Text.Json;
using EventStore.Client;
using MassTransit;
using MediatR;
using NHSv2.Appointments.Application.Services.Contracts;
using NHSv2.Appointments.Domain.Appointments;
using NHSv2.Appointments.Domain.Appointments.Events;
using NHSv2.Messaging.Contracts.MessageContracts;

namespace NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;

public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand>
{
    private readonly EventStoreClient _eventStoreClient;
    private readonly IBus _bus;
    private readonly ICalendarService _calendarService;

    public CreateAppointmentHandler(
        EventStoreClient eventStoreClient,
        IBus bus,
        ICalendarService calendarService)
    {
        _eventStoreClient = eventStoreClient;
        _bus = bus;
        _calendarService = calendarService;
    }
    
    public async Task Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var createdCalendarEvent = await _calendarService.CreateAppointmentAsync(
            request.Summary,
            request.Description,
            DateTime.Now,
            request.PatientEmail,
            request.FacilityName,
            request.DoctorName);
        
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(AppointmentCreatedEvent),
            JsonSerializer.SerializeToUtf8Bytes(new AppointmentCreatedEvent(new Appointment(Guid.NewGuid(),  Guid.NewGuid().ToString(), createdCalendarEvent.Id)))
        );
            
        await _eventStoreClient.AppendToStreamAsync(
            "appointments",
            StreamState.Any,
            new[] { eventData },
            cancellationToken: new CancellationToken()
        );
        
        await _bus.Publish(new AppointmentCreatedContract(
            Guid.NewGuid(),
            "Test",
            $"<p>Appointment created.</p><a href='{createdCalendarEvent.HtmlLink}'>View appointment</a>"),
            cancellationToken);
    }
}