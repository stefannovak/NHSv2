using MediatR;
using NHSv2.Payments.Application.DTOs;
using NHSv2.Payments.Application.DTOs.Responses;

namespace NHSv2.Payments.Application.Payments.Commands.CreateCheckout;

public record CreateCheckoutCommand(CreateCheckoutRequestDto CheckoutRequest) : IRequest<CheckoutResponse>;