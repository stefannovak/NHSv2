using System.ComponentModel.DataAnnotations;

namespace NHSv2.Payments.Domain.Models;

public class Product
{
    public string Name { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }

    [Length(3, 3)]
    public string Currency { get; set; } = "GBP";

    public int Quantity { get; set; } = 1;
}