using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;
using NHSv2.Appointments.Application.Appointments.Commands.UpdateAppointment;

namespace NHSv2.Appointments.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize(Roles = "patient")]
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        // random list of words
        var words = new List<string> { "Test", "Test2", "Test3", "Test4", "Test5" };
        var random = new Random();

        var appointmentId = Guid.NewGuid();
        var appointmentCreatedCommand = new CreateAppointmentCommand(appointmentId, words[random.Next(0, words.Count)]);
        await _mediator.Send(appointmentCreatedCommand);

        return Created();
    }

    [Authorize(Roles = "doctor")]
    [HttpGet("doctor")]
    public async Task<ActionResult> GetDoctor()
    {
        return Ok();
    }
}