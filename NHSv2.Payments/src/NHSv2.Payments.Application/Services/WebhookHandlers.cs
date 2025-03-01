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

    public Task HandlePaymentIntentCreated(PaymentIntent paymentIntent)
    {
        var transactionId = GetTransactionIdFromEvent(paymentIntent);
        var paymentCreatedEvent = new PaymentIntentCreatedEvent(
            transactionId,
            paymentIntent.Id,
            paymentIntent.Amount,
            paymentIntent.Currency,
            paymentIntent.Status,
            paymentIntent.CustomerId);
        
        return _eventStoreService.AppendPaymentIntentCreatedEventAsync(paymentCreatedEvent);
    }

    public Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
    {
        var transactionId = GetTransactionIdFromEvent(paymentIntent);
        var paymentIntentSucceededEvent = new PaymentIntentSucceededEvent(
            transactionId,
            paymentIntent.Id,
            paymentIntent.Amount,
            paymentIntent.Currency,
            paymentIntent.Status,
            paymentIntent.CustomerId);
        
        return _eventStoreService.AppendPaymentIntentSucceededEventAsync(paymentIntentSucceededEvent);
    }

    public Task HandlePaymentIntentPartiallyFunded(PaymentIntent paymentIntent)
    {
        var transactionId = GetTransactionIdFromEvent(paymentIntent);
        var paymentIntentPartiallyFundedEvent = new PaymentIntentPartiallyFundedEvent(
            transactionId,
            paymentIntent.Id,
            paymentIntent.Amount,
            paymentIntent.Currency,
            paymentIntent.Status,
            paymentIntent.CustomerId);
        
        return _eventStoreService.AppendPaymentIntentPartiallyFundedEventAsync(paymentIntentPartiallyFundedEvent);
    }

    public Task HandlePaymentIntentPaymentFailed(PaymentIntent paymentIntent)
    {
        var transactionId = GetTransactionIdFromEvent(paymentIntent);
        var paymentIntentPaymentFailedEvent = new PaymentIntentPaymentFailedEvent(
            transactionId,
            paymentIntent.Id,
            paymentIntent.Amount,
            paymentIntent.Currency,
            paymentIntent.Status,
            paymentIntent.CustomerId);
        
        return _eventStoreService.AppendPaymentIntentPaymentFailedEventAsync(paymentIntentPaymentFailedEvent);
    }

    public Task HandlePaymentProcessing(PaymentIntent paymentIntent)
    {
        var transactionId = GetTransactionIdFromEvent(paymentIntent);
        var paymentIntentProcessingEvent = new PaymentIntentProcessingEvent(
            transactionId,
            paymentIntent.Id,
            paymentIntent.Amount,
            paymentIntent.Currency,
            paymentIntent.Status,
            paymentIntent.CustomerId);
        
        return _eventStoreService.AppendPaymentIntentProcessingEventAsync(paymentIntentProcessingEvent);
    }

    public Task HandlePaymentIntentRequiresAction(PaymentIntent paymentIntent)
    {
        var transactionId = GetTransactionIdFromEvent(paymentIntent);
        var paymentIntentRequiresActionEvent = new PaymentIntentRequiresActionEvent(
            transactionId,
            paymentIntent.Id,
            paymentIntent.Amount,
            paymentIntent.Currency,
            paymentIntent.Status,
            paymentIntent.CustomerId);
        
        return _eventStoreService.AppendPaymentIntentRequiresActionEventAsync(paymentIntentRequiresActionEvent);
    }

    public Task HandlePaymentIntentCanceled(PaymentIntent paymentIntent)
    {
        var transactionId = GetTransactionIdFromEvent(paymentIntent);
        var paymentIntentCanceledEvent = new PaymentIntentCanceledEvent(
            transactionId,
            paymentIntent.Id,
            paymentIntent.Amount,
            paymentIntent.Currency,
            paymentIntent.Status,
            paymentIntent.CustomerId);
        
        return _eventStoreService.AppendPaymentIntentCanceledEventAsync(paymentIntentCanceledEvent);
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