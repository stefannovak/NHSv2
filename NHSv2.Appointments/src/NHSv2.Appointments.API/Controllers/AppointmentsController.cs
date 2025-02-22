using System.Diagnostics;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;
using NHSv2.Appointments.Application.Appointments.Queries.GetAppointments;
using NHSv2.Appointments.Application.Helpers;
using NHSv2.Appointments.Application.Services.Contracts;
using NHSv2.Appointments.Dtos.Requests;

namespace NHSv2.Appointments.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICalendarService _calendarService;
    private readonly IKeycloakService _keycloakService;

    public AppointmentsController(
        IMediator mediator,
        ICalendarService calendarService,
        IKeycloakService keycloakService)
    {
        _mediator = mediator;
        _calendarService = calendarService;
        _keycloakService = keycloakService;
    }
    
    /// <summary>
    /// Get appointments for a facility. Accessible only by a doctor. A doctor can only get appointments for a clinic they belong to.
    /// </summary>
    /// <param name="facilityName"></param>
    /// <returns>A cached response if appointments have not changed.</returns>
    [Authorize(Roles = "doctor")]
    [HttpGet("facilities/{facilityName}")]
    public async Task<ActionResult> GetAppointments([FromRoute] string facilityName)
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
        var couldParseGuid = Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out var doctorId);
        if (couldParseGuid == false)
        {
            return Problem("Doctor ID not found could not be found from token.");
        }
        
        var facilitiesDoctorBelongsTo = User.Claims.Where(c => c.Type == "medical_facilities");
        var doctorBelongsToRequestFacility = facilitiesDoctorBelongsTo.Any(x => x.Value == facilityName);
        if (!doctorBelongsToRequestFacility)
        {
            return Unauthorized("Doctor does not belong to the facility.");
        }
        
        var appointments = await _mediator.Send(new GetAppointmentsQuery(facilityName, doctorId));
        return Ok(appointments);
    }

    /// <summary>
    /// Create an appointment. Appointments are 30 minute slots. Accessible only by a doctor. A doctor can only
    /// create an appointment for a clinic they belong to. A Google calendar appointment will be created. An appointment is then
    /// committed to an event store, where it is picked up and processed. There may be delays between the appointment being
    /// created and the appointment appearing in a database.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "doctor")]
    [HttpPost]
    public async Task<ActionResult> CreateAppointment([FromBody] CreateAppointmentRequestDto request)
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
        var doctorName = User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        var doctorEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var facilitiesDoctorBelongsTo = User.Claims.Where(c => c.Type == "medical_facilities");
        var doctorBelongsToRequestFacility = facilitiesDoctorBelongsTo.Any(x => x.Value == request.FacilityName);
        var couldParseGuid = Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out var doctorId);
        if (doctorName == null || doctorEmail == null || couldParseGuid == false)
        {
            return Problem("Doctor name, email or ID not found could not be found from token.");
        }
        
        if (!doctorBelongsToRequestFacility)
        {
            return Unauthorized("Doctor does not belong to the facility.");
        }

        var user = await _keycloakService.GetUserById(request.PatientId);
        if (user is null)
        {
            return NotFound("Patient not found.");
        }

        var isAppointmentSlotAvailable = await _calendarService.IsAppointmentSlotAvailable(request.Start);
        if (!isAppointmentSlotAvailable)
        {
            return BadRequest("Appointment slot is not available. Ensure there is 30 minutes between appointments.");
        }
        
        var appointmentCreatedCommand = new CreateAppointmentCommand(
            request.Start,
            user.Email,
            request.Summary,
            request.Description,
            request.FacilityName,
            doctorName,
            request.PatientId,
            doctorId);
        
        await _mediator.Send(appointmentCreatedCommand);
        return Accepted();
    }
}