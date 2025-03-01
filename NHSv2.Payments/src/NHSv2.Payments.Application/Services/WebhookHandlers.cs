using EventStore.Client;
using NHSv2.Payments.Application.Services.Contracts;
using NHSv2.Payments.Domain.Transactions.Payments.Events.Stripe;
using Stripe;

namespace NHSv2.Payments.Application.Services;

public class WebhookHandlers : IWebhookHandlers
{
    private readonly IEventStoreService _eventStoreService;

    public WebhookHandlers(IEventStoreService eventStoreService)
    {
        _eventStoreService = eventStoreService;
    }

    public async Task HandlePaymentIntentCreated(PaymentIntent paymentIntent)
    {
        var transactionId = GetTransactionIdFromEvent(paymentIntent);
        var paymentCreatedEvent = new PaymentIntentCreatedEvent(
            transactionId,
            paymentIntent.Id,
            paymentIntent.Amount,
            paymentIntent.Currency,
            paymentIntent.Status,
            paymentIntent.CustomerId);
        
        await _eventStoreService.AppendPaymentIntentCreatedEventAsync(paymentCreatedEvent);
    }

    public async Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
    {
        var transactionId = GetTransactionIdFromEvent(paymentIntent);
        var paymentIntentSucceededEvent = new PaymentIntentSucceededEvent(
            transactionId,
            paymentIntent.Id,
            paymentIntent.Amount,
            paymentIntent.Currency,
            paymentIntent.Status,
            paymentIntent.CustomerId);
        
        await _eventStoreService.AppendPaymentIntentSucceededEventAsync(paymentIntentSucceededEvent);
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

    private Guid GetTransactionIdFromEvent(IHasMetadata eventData)
    {
        try
        {
            return Guid.Parse(eventData.Metadata["transactionId"]);
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not parse transactionId from event", e);
            throw;
        }
    }
}