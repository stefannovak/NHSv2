namespace NHSv2.Payments.Application.DTOs;

public record CreateCheckoutRequestDto(
    string CustomerEmail,
    IList<CheckoutProductDto> Products,
    string RedirectUrl,
    string ReturnUrl);