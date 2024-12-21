using System.Text.Json;
using EventStore.Client;
using MassTransit;
using MediatR;
using NHSv2.Appointments.Domain.Appointments;
using NHSv2.Appointments.Domain.Appointments.Events;
using NHSv2.Messaging.Contracts.MessageContracts;

namespace NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;

public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand>
{
    private readonly EventStoreClient _eventStoreClient;
    private readonly IBus _bus;

    public CreateAppointmentHandler(EventStoreClient eventStoreClient, IBus bus)
    {
        _eventStoreClient = eventStoreClient;
        _bus = bus;
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

        await _bus.Publish(new AppointmentCreatedContract(request.Id, "Test", "<p>Appointment created.</p>"), cancellationToken);
    }
}