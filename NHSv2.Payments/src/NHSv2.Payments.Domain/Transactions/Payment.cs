using NHSv2.Payments.Domain.Products;

namespace NHSv2.Payments.Domain.Transactions;

/// <summary>
/// Represents a one time payment per payment.
/// </summary>
public class Payment : Transaction
{
    public Guid ProductId { get; set; }

    // EF Core requires a parameterless constructor
    public Payment()
    {
    }
    
    public Payment(Product product)
    {
        TransactionId = Guid.NewGuid();
        Status = TransactionStatus.Pending;
        Type = TransactionType.OneTimePayment;
        Amount = product.Price;
        ProductId = product.ProductId;
    }
    
    public Payment(Product product, Guid transactionId)
    {
        TransactionId = transactionId;
        Status = TransactionStatus.Pending;
        Type = TransactionType.OneTimePayment;
        Amount = product.Price;
        ProductId = product.ProductId;
    }

    /// <summary>
    /// Returns a list of Payment objects with a shared TransactionId.
    /// A Checkout can include X items. Each item is a Payment object.
    /// Each Payment object has a shared TransactionId as they were all purchased in one transaction.
    /// </summary>
    /// <param name="products"></param>
    /// <returns></returns>
    public static (IList<Payment> payments, Guid transactionId) CreatePaymentsFromProducts(IEnumerable<Product> products)
    {
        var transactionId = Guid.NewGuid();
        return (products.Select(product => new Payment(product, transactionId)).ToList(), transactionId);
    }
}