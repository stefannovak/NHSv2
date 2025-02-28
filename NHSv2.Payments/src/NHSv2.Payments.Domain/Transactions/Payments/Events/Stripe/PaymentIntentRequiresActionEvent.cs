namespace NHSv2.Payments.Domain.Transactions.Payments.Events.Stripe;

public record PaymentIntentRequiresActionEvent(
    Guid TransactionId,
    string PaymentIntentId,
    decimal TotalAmount,
    string Currency,
    string Status,
    string CustomerId) : PaymentIntentBaseEvent(TransactionId, PaymentIntentId, TotalAmount, Currency, Status, CustomerId);