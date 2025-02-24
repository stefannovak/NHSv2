using System.Text.Json;
using EventStore.Client;
using MediatR;
using NHSv2.Payments.Application.DTOs.Responses;
using NHSv2.Payments.Application.Services.Contracts;
using NHSv2.Payments.Domain.Transactions;
using NHSv2.Payments.Domain.Transactions.Events;

namespace NHSv2.Payments.Application.Payments.Commands.CreateCheckout;

public class CreateCheckoutHandler: IRequestHandler<CreateCheckoutCommand, CheckoutResponse>
{
    private readonly ITransactionService _transactionService;
    private readonly EventStoreClient _eventStoreClient;

    public CreateCheckoutHandler(
        ITransactionService transactionService,
        EventStoreClient eventStoreClient)
    {
        _transactionService = transactionService;
        _eventStoreClient = eventStoreClient;
    }

    public async Task<CheckoutResponse> Handle(CreateCheckoutCommand request, CancellationToken cancellationToken)
    {
        var totalAmount = request.CheckoutRequest.Products.Sum(x => x.Amount * x.Quantity);
        var newTransaction = Transaction.CreateTransaction(totalAmount, TransactionType.OneTimePayment);

        var checkoutSession = await _transactionService.CreateCheckoutAsync(request.CheckoutRequest);
        
        var checkoutCreatedEvent = new TransactionCreated(newTransaction.TransactionId, request.CheckoutRequest.Products);
        var eventData = new EventData(
            Uuid.FromGuid(newTransaction.TransactionId),
            nameof(CreateCheckoutCommand),
            JsonSerializer.SerializeToUtf8Bytes(checkoutCreatedEvent));
            
        await _eventStoreClient.AppendToStreamAsync(
            "payments",
            StreamState.Any,
            new[] { eventData, },
            cancellationToken: cancellationToken
        );

        return checkoutSession;
    }
}