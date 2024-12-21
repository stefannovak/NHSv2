namespace NHSv2.Appointments.Domain.Appointments.Events;

public record AppointmentEmailSentEvent(Guid appointmentId);