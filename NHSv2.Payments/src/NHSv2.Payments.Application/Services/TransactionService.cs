using Microsoft.Extensions.Options;
using NHSv2.Payments.Application.Configurations;
using NHSv2.Payments.Application.DTOs;
using NHSv2.Payments.Application.DTOs.Responses;
using NHSv2.Payments.Application.Services.Contracts;
using Stripe.Checkout;

namespace NHSv2.Payments.Application.Services;

public class TransactionService : ITransactionService
{
    public TransactionService(IOptions<StripeConfiguration> options)
    {
        Stripe.StripeConfiguration.ApiKey = options.Value.ApiKey;
    }
    
    public async Task<CheckoutResponse> CreateCheckoutAsync(CreateCheckoutRequestDto request)
    {
        var checkout = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "gbp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "NHSv2 Payment",
                        },
                        UnitAmount = 1000,
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = "https://google.com",
            CancelUrl = "https://google.com",
        };
        
        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(checkout);

        Console.WriteLine(session.Url);
        return new CheckoutResponse(Guid.NewGuid(), session.Url);
    }
}