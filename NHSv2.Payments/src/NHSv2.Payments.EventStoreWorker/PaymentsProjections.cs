using System.Text.Json;
using EventStore.Client;
using Microsoft.Extensions.Caching.Distributed;
using NHSv2.Payments.Application.Helpers;
using NHSv2.Payments.Application.Repositories;
using NHSv2.Payments.Domain.Transactions;
using NHSv2.Payments.Domain.Transactions.Payments.Events;

namespace NHSv2.Payments.EventStoreWorker;

public class PaymentsProjections : BackgroundService
{
    private readonly ILogger<PaymentsProjections> _logger;
    private readonly EventStoreClient _eventStoreClient;
    private readonly IPaymentsRepository _paymentsRepository;
    // private readonly IEventStoreCheckpointRepository _checkpointRepository;
    // private readonly IDistributedCache _cache;
    private const string StreamName = "Payments";
    
    public PaymentsProjections(
        ILogger<PaymentsProjections> logger,
        EventStoreClient eventStoreClient,
        IPaymentsRepository paymentsRepository
        // IEventStoreCheckpointRepository checkpointRepository,
        // IDistributedCache cache)
        )
    {
        _logger = logger;
        _eventStoreClient = eventStoreClient;
        _paymentsRepository = paymentsRepository;
        // _checkpointRepository = checkpointRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // var checkpoint = await _checkpointRepository.GetCheckpoint(StreamName);
        var checkpoint = 0;
        
        await using var subscription = _eventStoreClient.SubscribeToStream(
            "$ce-payments",
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
        }
        
        // await _checkpointRepository.IncrementCheckpoint(StreamName);
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
        _logger.LogInformation($"Handling payment created: {paymentCreatedEvent.TransactionId}");
    }
    
    private async Task InsertPaymentToDatabase(PaymentCreatedEvent createdEvent)
    {
        var paymentTuples = Payment.CreatePaymentsFromProducts(createdEvent.Products);
        foreach (var payment in paymentTuples.payments)
        {
            await _paymentsRepository.InsertAsync(payment);
        }
        
        _logger.LogInformation($"Inserted payments {paymentTuples.transactionId} to database");
    }
}