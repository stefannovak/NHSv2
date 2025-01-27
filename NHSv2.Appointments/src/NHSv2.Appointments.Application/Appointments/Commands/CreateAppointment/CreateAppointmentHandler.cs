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
            "Appointment at Clinic with Dr",
            "Very important",
            DateTime.Now,
            string.Empty);
        
        // var eventData = new EventData(
        //     Uuid.NewUuid(),
        //     nameof(AppointmentCreatedEvent),
        //     JsonSerializer.SerializeToUtf8Bytes(new AppointmentCreatedEvent(new Appointment(request.Id, request.Test, createdCalendarEvent.Id)))
        // );
        //     
        // await _eventStoreClient.AppendToStreamAsync(
        //     "appointments",
        //     StreamState.Any,
        //     new[] { eventData },
        //     cancellationToken: new CancellationToken()
        // );
        //
        // await _bus.Publish(new AppointmentCreatedContract(
        //     request.Id,
        //     "Test",
        //     $"<p>Appointment created.</p><a href='{createdCalendarEvent.HtmlLink}'>View appointment</a>"),
        //     cancellationToken);
    }
}