namespace NHSv2.Appointments.Application.Dtos.Responses;

public class AppointmentDtoByDoctor
{
    public Guid PatientId { get; set; }

    public Guid AppointmentId { get; set; }

    public DateTimeOffset Start { get; set; }
    
    public string ClientCalendarId { get; init; }
}