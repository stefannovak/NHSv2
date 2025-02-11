using System.ComponentModel.DataAnnotations;

namespace NHSv2.Appointments.Domain.Appointments;

public class Appointment : BaseEntity
{
    public Appointment(
        Guid doctorId,
        Guid patientId,
        Guid appointmentId,
        DateTimeOffset start,
        string facilityName,
        string clientCalendarId)
    {
        DoctorId = doctorId;
        PatientId = patientId;
        AppointmentId = appointmentId;
        Start = start;
        FacilityName = facilityName;
        ClientCalendarId = clientCalendarId;
    }

    [Required]
    public Guid DoctorId { get; set; }
    
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public Guid AppointmentId { get; set; }

    [Required]
    public DateTimeOffset Start { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FacilityName { get; set; }

    /// <summary>
    /// Currently this is a Google Calendar Event id.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ClientCalendarId { get; init; }
}