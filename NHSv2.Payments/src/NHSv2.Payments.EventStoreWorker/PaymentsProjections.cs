using System.Text.Json;
using EventStore.Client;
using NHSv2.Payments.Application.Helpers;
using NHSv2.Payments.Application.Repositories;
using NHSv2.Payments.Domain.Transactions;
using NHSv2.Payments.Domain.Transactions.Payments.Events;
using NHSv2.Payments.Domain.Transactions.Payments.Events.Stripe;

namespace NHSv2.Payments.EventStoreWorker;

public class PaymentsProjections : BackgroundService
{
    private readonly EventStoreClient _eventStoreClient;
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IEventStoreCheckpointRepository _checkpointRepository;
    private const string StreamName = "$ce-payments";
    
    public PaymentsProjections(
        EventStoreClient eventStoreClient,
        IPaymentsRepository paymentsRepository,
        IEventStoreCheckpointRepository checkpointRepository)
    {
        _eventStoreClient = eventStoreClient;
        _paymentsRepository = paymentsRepository;
        _checkpointRepository = checkpointRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var checkpoint = await _checkpointRepository.GetCheckpoint(StreamName);
        
        await using var subscription = _eventStoreClient.SubscribeToStream(
            StreamName,
            FromStream.After(new StreamPosition(Convert.ToUInt32(checkpoint))),
            resolveLinkTos: true,
            cancellationToken: stoppingToken);
        
        await foreach (var message in subscription.Messages.WithCancellation(stoppingToken)) {
            switch (message) {
                case StreamMessage.Event(var evnt):
                    await HandleEvent(evnt);
                    break;
            }
        }
    }
    
    private async Task HandleEvent(ResolvedEvent evnt)
    {
        switch (evnt.Event.EventType)
        {
            case nameof(PaymentCreatedEvent):
                await HandlePaymentCreated(evnt);
                break;
            case nameof(PaymentIntentCanceledEvent):
                await HandlePaymentIntentCanceled(evnt);
                break;
            case nameof(PaymentIntentCreatedEvent):
                await HandlePaymentIntentCreated(evnt);
                break;
            case nameof(PaymentIntentPartiallyFundedEvent):
                await HandlePaymentIntentPartiallyFunded(evnt);
                break;
            case nameof(PaymentIntentPaymentFailedEvent):
                await HandlePaymentIntentPaymentFailed(evnt);
                break;
            case nameof(PaymentIntentProcessingEvent):
                await HandlePaymentIntentProcessing(evnt);
                break;
            case nameof(PaymentIntentRequiresActionEvent):
                await HandlePaymentIntentRequiresAction(evnt);
                break;
            case nameof(PaymentIntentSucceededEvent):
                await HandlePaymentIntentSucceeded(evnt);
                break;
            default:
                Console.WriteLine($"Failed to handle event {evnt.Event.EventType}");
                break;
        }
        
        await _checkpointRepository.IncrementCheckpoint(StreamName);
    }
    
    private async Task HandlePaymentCreated(ResolvedEvent evnt)
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
        var paymentCreatedEvent = JsonSerializer.Deserialize<PaymentCreatedEvent>(evnt.Event.Data.Span);
        if (paymentCreatedEvent == null)
        {
            activity?.LogExceptionEvent("FailedToDeserialize", new ArgumentException(), nameof(evnt));
            return;
        }

        activity?.AddTag(nameof(paymentCreatedEvent.TransactionId), paymentCreatedEvent.TransactionId);
        await InsertPaymentToDatabase(paymentCreatedEvent);
    }
    
    private Task HandlePaymentIntentCanceled(ResolvedEvent evnt)
    {
        return HandlePaymentIntentEvent<PaymentIntentCanceledEvent>(evnt, TransactionStatus.Canceled);
    }
    
    private Task HandlePaymentIntentCreated(ResolvedEvent evnt)
    {
        return HandlePaymentIntentEvent<PaymentIntentCreatedEvent>(evnt, TransactionStatus.Pending);
    }
    
    private Task HandlePaymentIntentPartiallyFunded(ResolvedEvent evnt)
    {
        return HandlePaymentIntentEvent<PaymentIntentPartiallyFundedEvent>(evnt, TransactionStatus.RequiresAction);
    }
    
    private Task HandlePaymentIntentPaymentFailed(ResolvedEvent evnt)
    {
        return HandlePaymentIntentEvent<PaymentIntentPaymentFailedEvent>(evnt, TransactionStatus.Failed);
    }
    
    private Task HandlePaymentIntentProcessing(ResolvedEvent evnt)
    {
        return HandlePaymentIntentEvent<PaymentIntentProcessingEvent>(evnt, TransactionStatus.Pending);
    }
    
    private Task HandlePaymentIntentRequiresAction(ResolvedEvent evnt)
    {
        return HandlePaymentIntentEvent<PaymentIntentRequiresActionEvent>(evnt, TransactionStatus.RequiresAction);
    }
    
    private Task HandlePaymentIntentSucceeded(ResolvedEvent evnt)
    {
        return HandlePaymentIntentEvent<PaymentIntentSucceededEvent>(evnt, TransactionStatus.Success);
    }

    private async Task HandlePaymentIntentEvent<T>(ResolvedEvent evnt, TransactionStatus status) where T : PaymentIntentBaseEvent
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
        var piEvent = JsonSerializer.Deserialize<T>(evnt.Event.Data.Span);
        if (piEvent == null)
        {
            activity?.LogExceptionEvent("FailedToDeserialize", new ArgumentException(), nameof(evnt));
            return;
        }

        activity?.AddTag(nameof(piEvent.TransactionId), piEvent.TransactionId);
        var payments = _paymentsRepository.GetPayments(piEvent.TransactionId).ToList();
        foreach (var payment in payments)
        {
            // TODO: - Properly handle events arriving out of order.
            if (payment.Status != TransactionStatus.Success)
            {
                payment.Status = status;
                await _paymentsRepository.UpdateAsync(payment);
            }
        }
    }

    private async Task InsertPaymentToDatabase(PaymentCreatedEvent createdEvent)
    {
        foreach (var product in createdEvent.Products)
        {
            var payment = new Payment(product, createdEvent.TransactionId);
            await _paymentsRepository.InsertAsync(payment);
        }
    }
}