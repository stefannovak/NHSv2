using NHSv2.Payments.Domain.Models;

namespace NHSv2.Payments.Domain.Transactions.Events;

public record TransactionCreated(Guid TransactionId, IList<Product> Products);