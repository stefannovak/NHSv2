using NHSv2.Payments.Domain.Transactions;

namespace NHSv2.Payments.Application.DTOs.Generic;

public record TransactionDto(
    Guid TransactionId,
    TransactionStatus Status,
    decimal Amount,
    Guid ProductId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);