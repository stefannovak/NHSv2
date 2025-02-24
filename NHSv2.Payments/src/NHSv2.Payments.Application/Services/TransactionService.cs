using Microsoft.Extensions.Options;
using NHSv2.Payments.Application.DTOs;
using NHSv2.Payments.Application.DTOs.Responses;
using NHSv2.Payments.Application.Services.Contracts;
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
    
    public async Task<CheckoutResponse> CreateCheckoutAsync(CreateCheckoutRequestDto request)
    {
        var customer = await GetCustomer(request.CustomerEmail);
        var lineItems = request.Products.Select(x => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = x.Currency,
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = x.Name,
                },
                UnitAmount = Convert.ToInt64(x.Amount)
            },
            Quantity = x.Quantity,
        });
        
        var checkout = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = lineItems.ToList(),
            Mode = "payment",
            SuccessUrl = request.RedirectUrl,
            CancelUrl = request.ReturnUrl,
            Customer = customer.Id,
        };
        
        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(checkout);

        Console.WriteLine(session.Url);
        return new CheckoutResponse(Guid.NewGuid(), session.Url);
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