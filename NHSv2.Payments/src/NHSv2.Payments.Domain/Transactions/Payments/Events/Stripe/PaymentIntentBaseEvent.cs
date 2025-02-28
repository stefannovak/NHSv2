namespace NHSv2.Payments.Domain.Transactions.Payments.Events.Stripe;

public record PaymentIntentBaseEvent(
    Guid TransactionId,
    string PaymentIntentId,
    decimal TotalAmount,
    string Currency,
    string Status,
    string CustomerId);