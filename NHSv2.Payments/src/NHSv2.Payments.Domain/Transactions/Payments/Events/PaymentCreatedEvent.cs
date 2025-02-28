using NHSv2.Payments.Domain.Products;

namespace NHSv2.Payments.Domain.Transactions.Payments.Events;

public record PaymentCreatedEvent(
    Guid TransactionId, 
    decimal TotalAmount,
    string Currency, 
    string CustomerId, 
    IReadOnlyCollection<Product> Products);