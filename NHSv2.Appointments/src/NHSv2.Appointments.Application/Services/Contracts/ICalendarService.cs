using Google.Apis.Calendar.v3.Data;

namespace NHSv2.Appointments.Application.Services.Contracts;

public interface ICalendarService
{
    Task<IList<TimePeriod>> GetAvailableSlotsAsync(DateTime start, DateTime end);

    /// <summary>
    /// Create a 30 minute appointment.
    /// </summary>
    /// <param name="summary"></param>
    /// <param name="description"></param>
    /// <param name="startTime"></param>
    /// <param name="patientEmail"></param>
    /// <returns></returns>
    Task<Event> CreateAppointmentAsync(string summary, string description, DateTime startTime, string patientEmail);
    
    /// <summary>
    /// Checks to see if there's any appointments in the next half hour from start.
    /// </summary>
    /// <param name="start">Appointment start time.</param>
    /// <returns></returns>
    Task<bool> IsAppointmentSlotAvailable(DateTime start);
}