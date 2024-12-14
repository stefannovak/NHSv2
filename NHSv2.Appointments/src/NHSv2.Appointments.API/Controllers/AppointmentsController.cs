using System.Text.Json;
using EventStore.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NHSv2.Appointments.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController(EventStoreClient eventStoreClient) : ControllerBase
{
    [Authorize(Roles = "patient")]
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        for (var i = 0; i < 100; i++)
        {
            var evt = new {
                EntityId      = Guid.NewGuid().ToString("N"),
                ImportantData = $"I wrote my first event! {i}"
            };

            var eventData = new EventData(
                Uuid.NewUuid(),
                "TestEvent",
                JsonSerializer.SerializeToUtf8Bytes(evt)
            );
            
            await eventStoreClient.AppendToStreamAsync(
                "some-stream",
                StreamState.Any,
                new[] { eventData },
                cancellationToken: new CancellationToken()
            );
        }
        
        return Ok();
    }

    [Authorize(Roles = "doctor")]
    [HttpGet("doctor")]
    public async Task<ActionResult> GetDoctor()
    {
        return Ok();
    }
}