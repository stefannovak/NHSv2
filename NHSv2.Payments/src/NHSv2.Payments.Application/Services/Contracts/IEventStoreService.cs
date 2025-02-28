using EventStore.Client;
using NHSv2.Payments.Domain.Transactions.Payments.Events;

namespace NHSv2.Payments.Application.Services.Contracts;

public interface IEventStoreService
{
    /// <summary>
    /// Append a PaymentIntentSucceededEvent to the event store in a transactionId specific stream.
    /// </summary>
    /// <param name="paymentCreatedEvent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IWriteResult> AppendPaymentCreatedEventAsync(
        PaymentCreatedEvent paymentCreatedEvent,
        CancellationToken cancellationToken = new());
}