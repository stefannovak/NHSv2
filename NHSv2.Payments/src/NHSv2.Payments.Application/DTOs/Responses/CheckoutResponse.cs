namespace NHSv2.Payments.Application.DTOs.Responses;

public record CheckoutResponse(Guid TransactionId, string RedirectUrl);