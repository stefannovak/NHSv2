using MediatR;
using NHSv2.Payments.Application.DTOs.Generic;
using NHSv2.Payments.Application.Services.Contracts;

namespace NHSv2.Payments.Application.Payments.Queries.GetPaymentsByTransactionId;

public class GetPaymentsByTransactionIdHandler : IRequestHandler<GetPaymentsByTransactionIdQuery, IReadOnlyCollection<TransactionDto>>
{
    private readonly ITransactionService _transactionService;

    public GetPaymentsByTransactionIdHandler(
        ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    
    public Task<IReadOnlyCollection<TransactionDto>> Handle(GetPaymentsByTransactionIdQuery request, CancellationToken cancellationToken)
    {
        return _transactionService.GetPaymentsByTransactionId(request.TransactionId);
    }
}