namespace NHSv2.Payments.Application.DTOs.Responses;    

public record CheckoutSessionResponseDto(Guid TransactionId, string RedirectUrl, string CustomerId);