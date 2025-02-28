using Stripe;

namespace NHSv2.Payments.Application.Services.Contracts;

public interface IWebhookHandlers
{
    Task HandlePaymentIntentCreated(PaymentIntent paymentIntent);
    
    Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent);
    
    Task HandlePaymentIntentPartiallyFunded(PaymentIntent paymentIntent);
    
    Task HandlePaymentIntentPaymentFailed(PaymentIntent paymentIntent);
    
    Task HandlePaymentProcessing(PaymentIntent paymentIntent);
    
    Task HandlePaymentIntentRequiresAction(PaymentIntent paymentIntent);
    
    Task HandlePaymentIntentCanceled(PaymentIntent paymentIntent);
}