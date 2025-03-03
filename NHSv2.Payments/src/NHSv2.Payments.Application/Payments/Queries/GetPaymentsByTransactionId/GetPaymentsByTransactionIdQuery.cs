using MediatR;
using NHSv2.Payments.Application.DTOs.Generic;

namespace NHSv2.Payments.Application.Payments.Queries.GetPaymentsByTransactionId;

public record GetPaymentsByTransactionIdQuery(Guid TransactionId) : IRequest<IReadOnlyCollection<TransactionDto>>;