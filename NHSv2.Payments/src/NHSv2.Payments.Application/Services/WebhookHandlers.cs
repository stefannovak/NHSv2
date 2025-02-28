using EventStore.Client;
using NHSv2.Payments.Application.Services.Contracts;
using Stripe;

namespace NHSv2.Payments.Application.Services;

public class WebhookHandlers : IWebhookHandlers
{
    private readonly EventStoreClient _eventStoreClient;

    public WebhookHandlers(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }

    public Task HandlePaymentIntentCreated(PaymentIntent paymentIntent)
    {
        throw new NotImplementedException();
    }

    public async Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
    {
        // var transactionConfirmedEvent = new TransactionConfirmedEvent(paymentIntent.Id);
        // var eventData = new EventStore.Client.EventData(
        //     Uuid.NewUuid(),
        //     nameof(TransactionConfirmedEvent),
        //     JsonSerializer.SerializeToUtf8Bytes(transactionConfirmedEvent));
        //
        // await _eventStoreClient.AppendToStreamAsync(
        //     "payments",
        //     StreamState.Any,
        //     new[] { eventData }
        // );
    }

    public Task HandlePaymentIntentPartiallyFunded(PaymentIntent paymentIntent)
    {
        throw new NotImplementedException();
    }

    public Task HandlePaymentIntentPaymentFailed(PaymentIntent paymentIntent)
    {
        throw new NotImplementedException();
    }

    public Task HandlePaymentProcessing(PaymentIntent paymentIntent)
    {
        throw new NotImplementedException();
    }

    public Task HandlePaymentIntentRequiresAction(PaymentIntent paymentIntent)
    {
        throw new NotImplementedException();
    }

    public Task HandlePaymentIntentCanceled(PaymentIntent paymentIntent)
    {
        throw new NotImplementedException();
    }
}