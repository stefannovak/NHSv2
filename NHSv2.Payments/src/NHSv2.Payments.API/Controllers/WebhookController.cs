using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NHSv2.Payments.Application.Services.Contracts;
using Stripe;
using StripeConfiguration = NHSv2.Payments.Application.Configurations.StripeConfiguration;

namespace NHSv2.Payments.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IOptions<StripeConfiguration> _options;
    private readonly IWebhookHandlers _webhookHandlers;

    public WebhookController(
        IOptions<StripeConfiguration> options,
        IWebhookHandlers webhookHandlers)
    {
        _options = options;
        _webhookHandlers = webhookHandlers;
    }
    
    [HttpPost]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _options.Value.WebhookSecret);

            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentCreated:
                    await _webhookHandlers.HandlePaymentIntentCreated((stripeEvent.Data.Object as PaymentIntent)!); 
                    break;
                case EventTypes.PaymentIntentSucceeded:
                    await _webhookHandlers.HandlePaymentIntentSucceeded((stripeEvent.Data.Object as PaymentIntent)!); 
                    break;
                default:
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                    break;
            }
            
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}