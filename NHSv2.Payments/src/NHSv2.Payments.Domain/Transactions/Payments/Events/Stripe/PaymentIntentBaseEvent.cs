namespace NHSv2.Payments.Domain.Transactions.Payments.Events.Stripe;

public record PaymentIntentBaseEvent(
    Guid TransactionId,
    string PaymentIntentId,
    decimal Amount,
    string Currency,
    string Status,
    string CustomerId);