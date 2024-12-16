namespace NHSv2.Appointments.Domain.Appointments.Events;

public record AppointmentUpdatedEvent(Guid id, string testUpdate);