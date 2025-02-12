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
}