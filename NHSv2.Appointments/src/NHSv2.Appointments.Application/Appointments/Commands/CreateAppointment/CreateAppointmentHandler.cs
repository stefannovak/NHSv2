using System.Text.Json;
using EventStore.Client;
using MassTransit;
using MediatR;
using NHSv2.Appointments.Application.Services.Contracts;
using NHSv2.Appointments.Domain.Appointments.Events;
using NHSv2.Messaging.Contracts.MessageContracts;

namespace NHSv2.Appointments.Application.Appointments.Commands.CreateAppointment;

public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand>
{
    private readonly EventStoreClient _eventStoreClient;
    private readonly IBus _bus;
    private readonly ICalendarService _calendarService;

    public CreateAppointmentHandler(
        EventStoreClient eventStoreClient,
        IBus bus,
        ICalendarService calendarService)
    {
        _eventStoreClient = eventStoreClient;
        _bus = bus;
        _calendarService = calendarService;
    }
    
    public async Task Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointmentId = Guid.NewGuid();
        
        // TODO: - I want to be able to attach NHSv2's appointmentId to this calendar implementation.
        var createdCalendarEvent = await _calendarService.CreateAppointmentAsync(
            request.Summary,
            request.Description,
            DateTime.Now,
            request.PatientEmail,
            request.FacilityName,
            request.DoctorName);

        var appointmentCreatedEvent = new AppointmentCreatedEvent(
            appointmentId,
            request.FacilityName,
            request.DoctorId,
            request.PatientId,
            request.Start,
            createdCalendarEvent.Id);
        
        var eventData = new EventData(
            Uuid.NewUuid(),
            nameof(AppointmentCreatedEvent),
            JsonSerializer.SerializeToUtf8Bytes(appointmentCreatedEvent));
            
        await _eventStoreClient.AppendToStreamAsync(
            "appointments",
            StreamState.Any,
            new[] { eventData },
            cancellationToken: cancellationToken
        );
        
        await _bus.Publish(new AppointmentCreatedContract(
            appointmentId,
            $"Your appointment at {request.FacilityName} has been confirmed.",
            EmailHtmlBody(request)),
            cancellationToken);
    }

    private string EmailHtmlBody(CreateAppointmentCommand request)
    {
        return $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    line-height: 1.6;
                }}
                .container {{
                    width: 80%;
                    margin: auto;
                    padding: 20px;
                    border: 1px solid #ccc;
                    border-radius: 10px;
                    background-color: #f9f9f9;
                }}
                .header {{
                    text-align: center;
                    padding-bottom: 20px;
                }}
                .details {{
                    margin-top: 20px;
                }}
                .details p {{
                    margin: 5px 0;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>Appointment Confirmation</h2>
                </div>
                <div class='details'>
                    <p>Dear {request.PatientEmail},</p>
                    <p>We are pleased to confirm your appointment with Dr. {request.DoctorName} at {request.FacilityName}.</p>
                    <p><strong>Appointment Details:</strong></p>
                    <p><strong>Date and Time:</strong> {request.Start.ToString("f")}</p>
                    <p><strong>Summary:</strong> {request.Summary}</p>
                    <p><strong>Description:</strong> {request.Description}</p>
                    <p>We look forward to seeing you then!</p>
                    <p>Best regards,</p>
                    <p>{request.FacilityName}</p>
                </div>
            </div>
        </body>
        </html>";
    }
}