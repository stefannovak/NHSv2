using NHSv2.Payments.Domain.Transactions;

namespace NHSv2.Payments.Application.Repositories;

public interface IPaymentsRepository
{
    /// <summary>
    /// Insert a payment into the Payments table.
    /// </summary>
    /// <param name="payment"></param>
    /// <returns></returns>
    Task InsertAsync(Payment payment);
    
    /// <summary>
    /// Get payments using a defined predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IEnumerable<Payment> GetPayments(Func<Payment, bool> predicate);
}