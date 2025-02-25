using Stripe;

namespace NHSv2.Payments.Application.Services.Contracts;

public interface IWebhookHandlers
{
    Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent);
}