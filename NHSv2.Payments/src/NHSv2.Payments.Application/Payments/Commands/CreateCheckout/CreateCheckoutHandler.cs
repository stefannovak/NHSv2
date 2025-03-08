using System.Text.Json;
using EventStore.Client;
using MediatR;
using NHSv2.Payments.Application.DTOs.Responses;
using NHSv2.Payments.Application.Services.Contracts;
using NHSv2.Payments.Domain.Products;
using NHSv2.Payments.Domain.Transactions;
using NHSv2.Payments.Domain.Transactions.Payments.Events;

namespace NHSv2.Payments.Application.Payments.Commands.CreateCheckout;

public class CreateCheckoutHandler: IRequestHandler<CreateCheckoutCommand, CheckoutResponseDto>
{
    private readonly ITransactionService _transactionService;
    private readonly IEventStoreService _eventStoreService;

    public CreateCheckoutHandler(
        ITransactionService transactionService,
        IEventStoreService eventStoreService)
    {
        _transactionService = transactionService;
        _eventStoreService = eventStoreService;
    }

    public async Task<CheckoutResponseDto> Handle(CreateCheckoutCommand request, CancellationToken cancellationToken)
    {

        var products = new List<Product>();
        foreach (var product in request.CheckoutRequest.Products)
        {
            for (int i = 0; i < product.Quantity; i++)
            {
                // We'd get the productId from metadata or a products microservice before the checkout is made.
                var productId = Guid.NewGuid();
                products.Add(new Product(productId, product.Name, product.PriceInLowestDenominator));
            }
        }
        
        var payments = Payment.CreatePaymentsFromProducts(products);
        var checkoutSession = await _transactionService.CreateCheckoutAsync(request.CheckoutRequest, payments.transactionId);
        
        var totalAmount = request.CheckoutRequest.Products.Sum(x => x.PriceInLowestDenominator * x.Quantity);
        var paymentCreatedEvent = new PaymentCreatedEvent(
            payments.transactionId,
            totalAmount,
            request.CheckoutRequest.Products.Select(x => x.Currency).First(),
            checkoutSession.CustomerId,
            products);

        await _eventStoreService.AppendPaymentCreatedEventAsync(paymentCreatedEvent, cancellationToken);
        return new CheckoutResponseDto(checkoutSession.TransactionId, checkoutSession.RedirectUrl);
    }
}