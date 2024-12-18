using System.Text.Json;
using EventStore.Client;
using MediatR;
using NHSv2.Appointments.Domain.Appointments.Events;

namespace NHSv2.Appointments.Application.Appointments.Commands.UpdateAppointment;

public class UpdateAppointmentHandler : IRequestHandler<UpdateAppointmentCommand>
{
    private readonly EventStoreClient _eventStoreClient;

    public UpdateAppointmentHandler(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }

    public async Task Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var eventData2 = new EventData(
            Uuid.NewUuid(),
            nameof(AppointmentUpdatedEvent),
            JsonSerializer.SerializeToUtf8Bytes(new AppointmentUpdatedEvent(request.Id, request.Test))
        );

        await _eventStoreClient.AppendToStreamAsync(
            "appointments",
            StreamState.Any,
            new[] { eventData2 },
            cancellationToken: new CancellationToken()
        );
    }
}