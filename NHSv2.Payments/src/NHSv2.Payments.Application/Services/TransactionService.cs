using Microsoft.Extensions.Options;
using NHSv2.Payments.Application.DTOs;
using NHSv2.Payments.Application.DTOs.Responses;
using NHSv2.Payments.Application.Services.Contracts;
using NHSv2.Payments.Domain.Transactions;
using Stripe;
using Stripe.Checkout;
using StripeConfiguration = NHSv2.Payments.Application.Configurations.StripeConfiguration;

namespace NHSv2.Payments.Application.Services;

public class TransactionService : ITransactionService
{
    public TransactionService(IOptions<StripeConfiguration> options)
    {
        Stripe.StripeConfiguration.ApiKey = options.Value.ApiKey;
    }
    
    public async Task<CheckoutSessionResponseDto> CreateCheckoutAsync(CreateCheckoutRequestDto request, Guid transactionId)
    {
        var customer = await GetCustomer(request.CustomerEmail);
        
        var checkout = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = CreateLineItems(request),
            Mode = "payment",
            SuccessUrl = request.RedirectUrl,
            CancelUrl = request.ReturnUrl,
            Customer = customer.Id,
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    {"transactionId", transactionId.ToString()}
                }
            }
        };
        
        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(checkout);
        Console.WriteLine(session.Url);
        return new CheckoutSessionResponseDto(transactionId, session.Url, customer.Id);
    }

    private static List<SessionLineItemOptions> CreateLineItems(CreateCheckoutRequestDto request)
    {
        return request.Products.Select(x => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = x.Currency,
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = x.Name,
                },
                UnitAmountDecimal = x.PriceInLowestDenominator
            },
            Quantity = x.Quantity,
        }).ToList();
    }

    private async Task<Customer> GetCustomer(string customerEmail)
    {
        var customerService = new CustomerService();
        var customer = (await customerService.ListAsync(new CustomerListOptions
        {
            Email = customerEmail,
            Limit = 1,
        })).FirstOrDefault();

        if (customer is null)
        {
            customer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Email = customerEmail,
            });
                
            return customer;
        }
        
        return customer;
    }
}