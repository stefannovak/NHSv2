using System.ComponentModel.DataAnnotations;

namespace NHSv2.Payments.Application.DTOs;

public class CheckoutProductDto
{
    public string Name { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }

    [Length(3, 3)]
    public string Currency { get; set; } = "GDP";

    public int Quantity { get; set; } = 1;
}