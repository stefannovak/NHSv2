namespace NHSv2.Payments.Application.DTOs;

public class CheckoutProductDto
{
    public string Name { get; set; } = string.Empty;

    public string Currency { get; set; } = "GBP";
    
    public int PriceInLowestDenominator { get; set; }

    public int Quantity { get; set; } = 1;
}