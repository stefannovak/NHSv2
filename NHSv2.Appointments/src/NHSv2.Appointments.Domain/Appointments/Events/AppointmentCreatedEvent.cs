namespace NHSv2.Appointments.Domain.Appointments.Events;

public record AppointmentCreatedEvent(
    Guid AppointmentId, 
    string FacilityName,
    Guid DoctorId,
    Guid PatientId,
    DateTime AppointmentStart,
    string CalendarEventId);