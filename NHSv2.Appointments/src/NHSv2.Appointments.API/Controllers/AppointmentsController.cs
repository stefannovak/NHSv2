using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;
using NHSv2.Appointments.Application.Services.Contracts;

namespace NHSv2.Appointments.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICalendarService _calendarService;

    public AppointmentsController(IMediator mediator, ICalendarService calendarService)
    {
        _mediator = mediator;
        _calendarService = calendarService;
    }

    [Authorize(Roles = "doctor")]
    [HttpPost]
    public async Task<ActionResult> CreateAppointment()
    {
        var isAppointmentSlotAvailable = await _calendarService.IsAppointmentSlotAvailable(DateTime.Now);
        if (!isAppointmentSlotAvailable)
        {
            return BadRequest("Appointment slot is not available. Ensure there is 30 minutes between appointments.");
        }
        
        var appointmentCreatedCommand = new CreateAppointmentCommand(Guid.NewGuid(), "Test");
        await _mediator.Send(appointmentCreatedCommand);
        return Ok();
    }
}