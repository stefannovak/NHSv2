namespace NHSv2.Payments.Domain.Transactions.Payments.Events.Stripe;

public record PaymentIntentPaymentFailedEvent(
    Guid TransactionId,
    string PaymentIntentId,
    decimal Amount,
    string Currency,
    string Status,
    string CustomerId) : PaymentIntentBaseEvent(TransactionId, PaymentIntentId, Amount, Currency, Status, CustomerId);