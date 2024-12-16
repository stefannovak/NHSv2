using System.Text.Json;
using EventStore.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSv2.Appointments.Domain.Appointments;
using NHSv2.Appointments.Domain.Appointments.Events;

namespace NHSv2.Appointments.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController(EventStoreClient eventStoreClient) : ControllerBase
{
    // [Authorize(Roles = "patient")]
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var appointmentId = Guid.NewGuid();
        var appointmentCreatedEvent = new AppointmentCreatedEvent(new Appointment(appointmentId, "Test"));
        
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(AppointmentCreatedEvent),
            JsonSerializer.SerializeToUtf8Bytes(appointmentCreatedEvent)
        );
        
        await eventStoreClient.AppendToStreamAsync(
            "appointments",
            StreamState.Any,
            new[] { eventData },
            cancellationToken: new CancellationToken()
        );
        
        Thread.Sleep(5000);

        var appointmentUpdatedEvent = new AppointmentUpdatedEvent(appointmentId, "TestUpdate");
        var eventData2 = new EventData(
            Uuid.NewUuid(),
            nameof(AppointmentUpdatedEvent),
            JsonSerializer.SerializeToUtf8Bytes(appointmentUpdatedEvent)
        );
        
        await eventStoreClient.AppendToStreamAsync(
            "appointments",
            StreamState.Any,
            new[] { eventData2 },
            cancellationToken: new CancellationToken()
        );
        
        return Ok();
    }

    [Authorize(Roles = "doctor")]
    [HttpGet("doctor")]
    public async Task<ActionResult> GetDoctor()
    {
        return Ok();
    }
}