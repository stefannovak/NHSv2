namespace NHSv2.Payments.Application.Configurations;

public class StripeConfiguration
{
    public const string Stripe = "Stripe";

    public string ApiKey { get; set; } = string.Empty;
    
    public string WebhookSecret { get; set; } = string.Empty;
}