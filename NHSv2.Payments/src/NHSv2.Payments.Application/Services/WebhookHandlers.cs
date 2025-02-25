using System.Text.Json;
using EventStore.Client;
using NHSv2.Payments.Application.Services.Contracts;
using NHSv2.Payments.Domain.Transactions.Events;
using Stripe;

namespace NHSv2.Payments.Application.Services;

public class WebhookHandlers : IWebhookHandlers
{
    private readonly EventStoreClient _eventStoreClient;

    public WebhookHandlers(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }
    
    public async Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
    {
        var transactionConfirmedEvent = new TransactionConfirmedEvent(paymentIntent.Id);
        var eventData = new EventStore.Client.EventData(
            Uuid.NewUuid(),
            nameof(TransactionConfirmedEvent),
            JsonSerializer.SerializeToUtf8Bytes(transactionConfirmedEvent));

        await _eventStoreClient.AppendToStreamAsync(
            "payments",
            StreamState.Any,
            new[] { eventData }
        );
    }
}