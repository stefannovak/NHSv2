namespace NHSv2.Payments.Application.DTOs;

public class CheckoutProductAndQuantityDto
{
    public CheckoutProductAndQuantityDto(CheckoutProductDto product)
    {
        Product = product;
    }

    public CheckoutProductDto Product { get; set; }

    public int Quantity { get; set; } = 1;
}