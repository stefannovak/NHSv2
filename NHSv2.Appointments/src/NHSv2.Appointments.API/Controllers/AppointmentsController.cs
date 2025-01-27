using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;
using NHSv2.Appointments.Application.Services.Contracts;
using NHSv2.Appointments.Dtos.Requests;

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

    /// <summary>
    /// Create an appointment.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "doctor")]
    [HttpPost]
    public async Task<ActionResult> CreateAppointment([FromBody] CreateAppointmentRequestDto request)
    {
        var isAppointmentSlotAvailable = await _calendarService.IsAppointmentSlotAvailable(request.Start);
        if (!isAppointmentSlotAvailable)
        {
            return BadRequest("Appointment slot is not available. Ensure there is 30 minutes between appointments.");
        }
        
        var doctorName = User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        var doctorEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (doctorName == null || doctorEmail == null)
        {
            return Problem("Doctor name, email or clinic not found could not be found from token.");
        }
        
        var appointmentCreatedCommand = new CreateAppointmentCommand(
            request.Start,
            request.End,
            request.PatientId,
            request.Description);
        
        await _mediator.Send(appointmentCreatedCommand);
        return Accepted();
    }
}