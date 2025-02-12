using NHSv2.Appointments.Domain.Appointments;

namespace NHSv2.Appointments.Application.Repositories;

public interface IAppointmentsRepository
{
    /// <summary>
    /// Create an appointment.
    /// </summary>
    /// <param name="appointment"></param>
    /// <returns></returns>
    Task InsertAsync(Appointment appointment);
    
    /// <summary>
    /// Get appointments.
    /// </summary>
    /// <param name="predicate">A selector.</param>
    /// <returns></returns>
    IEnumerable<Appointment> GetAppointments(Func<Appointment, bool> predicate);
}