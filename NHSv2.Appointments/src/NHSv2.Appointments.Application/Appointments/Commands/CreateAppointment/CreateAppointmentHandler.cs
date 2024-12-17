using System.Text.Json;
using EventStore.Client;
using MediatR;
using NHSv2.Appointments.Domain.Appointments;
using NHSv2.Appointments.Domain.Appointments.Events;

namespace NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;

public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand>
{
    private readonly EventStoreClient _eventStoreClient;

    public CreateAppointmentHandler(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }
    
    public async Task Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(AppointmentCreatedEvent),
            JsonSerializer.SerializeToUtf8Bytes(new AppointmentCreatedEvent(new Appointment(request.Id, request.Test)))
        );
            
        await _eventStoreClient.AppendToStreamAsync(
            "appointments",
            StreamState.Any,
            new[] { eventData },
            cancellationToken: new CancellationToken()
        );
    }
}