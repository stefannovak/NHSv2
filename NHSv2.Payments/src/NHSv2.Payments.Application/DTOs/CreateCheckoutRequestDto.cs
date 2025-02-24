using NHSv2.Payments.Domain.Models;

namespace NHSv2.Payments.Application.DTOs;

public record CreateCheckoutRequestDto(
    string CustomerEmail,
    IList<Product> Products,
    string RedirectUrl,
    string ReturnUrl);