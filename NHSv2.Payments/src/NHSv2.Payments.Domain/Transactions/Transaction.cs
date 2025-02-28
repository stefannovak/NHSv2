using System.ComponentModel.DataAnnotations;

namespace NHSv2.Payments.Domain.Transactions;

public abstract class Transaction : BaseEntity
{
    public Guid TransactionId { get; set; }

    /// <summary>
    /// Amount comes from the product. A user may pay for multiple products. This Amount represents a single amount per product.
    /// </summary>
    public decimal Amount { get; set; }

    [Length(3, 3)] 
    public string Currency { get; set; } = "GBP";

    public TransactionStatus Status { get; set; }

    public TransactionType Type { get; set; }
}