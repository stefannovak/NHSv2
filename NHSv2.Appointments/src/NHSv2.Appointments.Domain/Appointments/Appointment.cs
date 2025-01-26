namespace NHSv2.Appointments.Domain.Appointments;

public class Appointment
{
    public Guid Id { get; private init; }

    public string Test { get; set; }

    /// <summary>
    /// Currently this is a Google Calendar Event id.
    /// </summary>
    public string AppointmentId { get; init; }

    public Appointment(Guid id, string test, string appointmentId)
    {
        Id = id;
        Test = test;
        AppointmentId = appointmentId;
    }
}