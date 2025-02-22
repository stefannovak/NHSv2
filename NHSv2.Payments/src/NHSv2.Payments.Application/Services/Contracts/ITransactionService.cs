using NHSv2.Payments.Application.DTOs;
using NHSv2.Payments.Application.DTOs.Responses;

namespace NHSv2.Payments.Application.Services.Contracts;

public interface ITransactionService
{
    /// <summary>
    /// Create a new checkout.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>TransactionId and a redirect URL.</returns>
    Task<CheckoutResponse> CreateCheckoutAsync(CreateCheckoutRequestDto request);
}