using NHSv2.Payments.Application.Helpers;
using NHSv2.Payments.Application.Repositories;
using NHSv2.Payments.Domain.Transactions;
using NHSv2.Payments.Infrastructure.Data;

namespace NHSv2.Payments.Infrastructure.Persistence;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly TransactionsDbContext _context;

    public PaymentsRepository(TransactionsDbContext context)
    {
        _context = context;
    }

    public async Task InsertAsync(Payment payment)
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }
    
    public IEnumerable<Payment> GetPayments(Func<Payment, bool> predicate)
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
        return _context.Payments.Where(predicate);
    }
}