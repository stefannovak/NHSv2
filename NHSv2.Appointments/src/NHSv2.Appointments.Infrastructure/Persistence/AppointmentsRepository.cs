using NHSv2.Appointments.Application.Helpers;
using NHSv2.Appointments.Application.Repositories;
using NHSv2.Appointments.Domain.Appointments;
using NHSv2.Appointments.Infrastructure.Data;

namespace NHSv2.Appointments.Infrastructure.Persistence;

public class AppointmentsRepository : IAppointmentsRepository
{
    private readonly AppointmentsDbContext _context;

    public AppointmentsRepository(AppointmentsDbContext context)
    {
        _context = context;
    }

    public async Task InsertAsync(Appointment appointment)
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
    }
    
    public IEnumerable<Appointment> GetAppointments(Func<Appointment, bool> predicate)
    {
        using var activity = ActivitySourceHelper.ActivitySource.StartActivity();
        return _context.Appointments.Where(predicate);
    }
}