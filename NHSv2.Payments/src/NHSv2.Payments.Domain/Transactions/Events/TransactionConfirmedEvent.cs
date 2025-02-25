namespace NHSv2.Payments.Domain.Transactions.Events;

public record TransactionConfirmedEvent(string PaymentIntentId);