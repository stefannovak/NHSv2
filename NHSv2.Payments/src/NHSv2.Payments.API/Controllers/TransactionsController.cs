using MediatR;
using Microsoft.AspNetCore.Mvc;
using NHSv2.Payments.Application.DTOs;
using NHSv2.Payments.Application.Payments.Commands.CreateCheckout;
using NHSv2.Payments.Application.Services.Contracts;

namespace NHSv2.Payments.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("one-time")]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateCheckoutRequestDto request)
    {
        // ValidationContext stuff
        var checkoutSession = await _mediator.Send(new CreateCheckoutCommand(request));
        
        return Ok(checkoutSession);
    }
}