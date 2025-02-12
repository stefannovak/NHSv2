using Microsoft.EntityFrameworkCore;
using NHSv2.Appointments.Application.Repositories;
using NHSv2.Appointments.Infrastructure.Data;

namespace NHSv2.Appointments.Infrastructure.Persistence;

public class EventStoreCheckpointRepository : IEventStoreCheckpointRepository
{
    private readonly AppointmentsDbContext _context;

    public EventStoreCheckpointRepository(AppointmentsDbContext context)
    {
        _context = context;
    }
    
    public async Task<long> GetCheckpoint(string streamName)
    {
        var checkpoint = await _context.EventStoreCheckpoints.FirstOrDefaultAsync(x => x.StreamName == streamName);
        return checkpoint?.Position ?? 0;
    }

    public async Task IncrementCheckpoint(string streamName)
    {
        var checkpoint = _context.EventStoreCheckpoints.First(x => x.StreamName == streamName);
        checkpoint.Position++;
        await _context.SaveChangesAsync();
    }
}