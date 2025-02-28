using EventStore.Client;
using MediatR;
using NHSv2.Payments.Application.DTOs.Responses;
using NHSv2.Payments.Application.Services.Contracts;
using NHSv2.Payments.Domain.Products;
using NHSv2.Payments.Domain.Transactions;

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
        // var totalAmount = request.CheckoutRequest.Products.Sum(x => x.Price * x.Quantity);

        // var products = new List<Product>();
        // foreach (var product in request.CheckoutRequest.Products)
        // {
        //     for (int i = 0; i < product.Quantity; i++)
        //     {
        //         // We'd get the productId from metadata or a products microservice before the checkout is made.
        //         var productId = Guid.NewGuid();
        //         products.Add(new Product(productId, product.Name, product.Price));
        //     }
        // }
        
        // We'd get the productId from metadata or a products microservice before the checkout is made.
        var products = request.CheckoutRequest.Products
            .SelectMany(product => Enumerable.Range(0, product.Quantity)
                .Select(_ => new Product(Guid.NewGuid(), product.Name, product.Price)))
            .ToList();
        
        var payments = Payment.CreatePaymentsFromProducts(products);
        var checkoutSession = await _transactionService.CreateCheckoutAsync(request.CheckoutRequest, payments.transactionId);
        
        // await AddCheckoutSessionToEventStore(payments, transactionId);
        
        //
        // var checkoutCreatedEvent = new TransactionCreated(newTransaction.TransactionId, request.CheckoutRequest.Products);
        // var eventData = new EventData(
        //     Uuid.FromGuid(newTransaction.TransactionId),
        //     nameof(CreateCheckoutCommand),
        //     JsonSerializer.SerializeToUtf8Bytes(checkoutCreatedEvent));
        //     
        // await _eventStoreClient.AppendToStreamAsync(
        //     "payments",
        //     StreamState.Any,
        //     new[] { eventData, },
        //     cancellationToken: cancellationToken
        // );

        return checkoutSession;
    }
}