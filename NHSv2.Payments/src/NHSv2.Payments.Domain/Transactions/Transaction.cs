using System.ComponentModel.DataAnnotations;

namespace NHSv2.Payments.Domain.Transactions;

public class Transaction : BaseEntity
{
    public Guid TransactionId { get; set; }

    public decimal Amount { get; set; }
 
    [Length(3,3)]
    public string Currency { get; set; }

    public TransactionStatus Status { get; set; }

    public TransactionType Type { get; set; }

    public static Transaction CreateTransaction(decimal amount, TransactionType type, string currency = "GBP")
    {
        return new Transaction
        {
            TransactionId = Guid.NewGuid(),
            Amount = amount,
            Status = TransactionStatus.Pending,
            Type = type,
            Currency = currency
        };
    }
}