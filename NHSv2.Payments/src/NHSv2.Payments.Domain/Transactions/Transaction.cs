using System.ComponentModel.DataAnnotations;

namespace NHSv2.Payments.Domain.Transactions;

public class Transaction : BaseEntity
{
    public Guid TransactionId { get; set; }

    public decimal Amount { get; set; }
 
    [Length(3,3)]
    public string Currency { get; set; } = "GBP";

    public TransactionStatus Status { get; set; }

    public TransactionType Type { get; set; }
}