using System.Text.Json;
using EventStore.Client;
using MassTransit;
using Microsoft.Extensions.Logging;
using NHSv2.Appointments.Domain.Appointments.Events;
using NHSv2.Messaging.Contracts.MessageContracts;

namespace NHSv2.Appointments.Application.Consumers;

public class AppointmentEmailSentConsumer : IConsumer<AppointmentEmailSentContract>
{
    private readonly ILogger<AppointmentEmailSentConsumer> _logger;
    private readonly EventStoreClient _eventStoreClient;

    public AppointmentEmailSentConsumer(ILogger<AppointmentEmailSentConsumer> logger, EventStoreClient eventStoreClient)
    {
        _logger = logger;
        _eventStoreClient = eventStoreClient;
    }
    
    public async Task Consume(ConsumeContext<AppointmentEmailSentContract> context)
    {
        _logger.LogInformation($"Email confirmed sent for appointment: {context.Message.AppointmentId}");
        
        // This isn't used at all, just cool.
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(AppointmentEmailSentEvent),
            JsonSerializer.SerializeToUtf8Bytes(new AppointmentEmailSentEvent(context.Message.AppointmentId))
        );
            
        await _eventStoreClient.AppendToStreamAsync(
            "emails",
            StreamState.Any,
            new[] { eventData },
            cancellationToken: new CancellationToken()
        );
    }
}