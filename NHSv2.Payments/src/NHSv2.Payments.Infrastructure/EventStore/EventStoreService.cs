using System.Text.Json;
using EventStore.Client;
using NHSv2.Payments.Application.Services.Contracts;
using NHSv2.Payments.Domain.Transactions.Payments.Events;
using NHSv2.Payments.Domain.Transactions.Payments.Events.Stripe;

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

    public async Task<IWriteResult> AppendPaymentIntentCreatedEventAsync(
        PaymentIntentCreatedEvent paymentIntentCreatedEvent,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(PaymentIntentCreatedEvent),
            JsonSerializer.SerializeToUtf8Bytes(paymentIntentCreatedEvent));
        
        return await _eventStoreClient.AppendToStreamAsync(
            GetPaymentsStreamName(paymentIntentCreatedEvent.TransactionId),
            StreamState.Any,
            new[] { eventData },
            cancellationToken: cancellationToken
        );
    }

    public async Task<IWriteResult> AppendPaymentIntentEvent(
        PaymentIntentBaseEvent baseEvent,
        CancellationToken cancellationToken)
    {
        var eventData = new EventData(
            Uuid.NewUuid(),
            baseEvent.GetType().Name,
            JsonSerializer.SerializeToUtf8Bytes(baseEvent));
        
        return await _eventStoreClient.AppendToStreamAsync(
            GetPaymentsStreamName(baseEvent.TransactionId),
            StreamState.Any,
            new[] { eventData },
            cancellationToken: cancellationToken
        );
    }

    private string GetPaymentsStreamName(Guid transactionId) => $"{PaymentsStreamName}-{transactionId}";
}