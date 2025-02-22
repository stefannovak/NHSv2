using Microsoft.AspNetCore.Mvc;
using NHSv2.Payments.Application.DTOs;
using NHSv2.Payments.Application.Services.Contracts;

namespace NHSv2.Payments.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateCheckoutRequestDto request)
    {
        // ValidationContext stuff
        
        var checkoutSession = await _transactionService.CreateCheckoutAsync(request);
        
        return Ok(checkoutSession);
    }
}