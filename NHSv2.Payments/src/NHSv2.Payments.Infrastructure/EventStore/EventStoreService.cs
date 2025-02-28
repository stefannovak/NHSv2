using System.Text.Json;
using EventStore.Client;
using NHSv2.Payments.Application.Services.Contracts;
using NHSv2.Payments.Domain.Transactions.Payments.Events;

namespace NHSv2.Payments.Infrastructure.EventStore;

public class EventStoreService : IEventStoreService
{
    private readonly EventStoreClient _eventStoreClient;
    private const string PaymentsStreamName = "payments";

    public EventStoreService(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }

    public async Task<IWriteResult> AppendPaymentCreatedEventAsync(PaymentCreatedEvent paymentCreatedEvent, CancellationToken cancellationToken = new ())
    {
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(PaymentCreatedEvent),
            JsonSerializer.SerializeToUtf8Bytes(paymentCreatedEvent));
        
        return await _eventStoreClient.AppendToStreamAsync(
            GetPaymentsStreamName(paymentCreatedEvent.TransactionId),
            StreamState.Any,
            new[] { eventData },
            cancellationToken: cancellationToken
        );
    }
    
    private string GetPaymentsStreamName(Guid transactionId) => $"{PaymentsStreamName}-{transactionId}";
}