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
        var doctorName = User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        var doctorEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var facilitiesDoctorBelongsTo = User.Claims.Where(c => c.Type == "medical_facilities");
        var doctorBelongsToRequestFacility = facilitiesDoctorBelongsTo.Any(x => x.Value == request.FacilityName);
        if (doctorName == null || doctorEmail == null)
        {
            return Problem("Doctor name or email not found could not be found from token.");
        }
        
        if (!doctorBelongsToRequestFacility)
        {
            return Unauthorized("Doctor does not belong to the facility.");
        }

        // TODO: - Keycloak service feature. 
        // var user = _keycloakService.GetUserById(request.PatientId);
        // if (user is null)
        // {
        //     return NotFound("Patient not found.");
        // }

        var isAppointmentSlotAvailable = await _calendarService.IsAppointmentSlotAvailable(request.Start);
        if (!isAppointmentSlotAvailable)
        {
            return BadRequest("Appointment slot is not available. Ensure there is 30 minutes between appointments.");
        }
        
        var appointmentCreatedCommand = new CreateAppointmentCommand(
            request.Start,
            // user.Email,
            "test@test.com",
            request.Summary,
            request.Description,
            request.FacilityName,
            doctorName);
        
        await _mediator.Send(appointmentCreatedCommand);
        return Accepted();
    }
}