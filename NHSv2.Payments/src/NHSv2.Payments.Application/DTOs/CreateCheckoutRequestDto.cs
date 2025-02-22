namespace NHSv2.Payments.Application.DTOs;

public record CreateCheckoutRequestDto(
    string CustomerEmail,
    IList<CheckoutProductAndQuantityDto> Products,
    string RedirectUrl,
    string ReturnUrl);