namespace NHSv2.Payments.Application.DTOs.Responses;

public record CheckoutResponseDto(Guid TransactionId, string RedirectUrl);