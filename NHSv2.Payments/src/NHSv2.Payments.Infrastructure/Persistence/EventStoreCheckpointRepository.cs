using Microsoft.EntityFrameworkCore;
using NHSv2.Payments.Application.Repositories;
using NHSv2.Payments.Infrastructure.Data;

namespace NHSv2.Payments.Infrastructure.Persistence;

public class EventStoreCheckpointRepository : IEventStoreCheckpointRepository
{
    private readonly TransactionsDbContext _context;

    public EventStoreCheckpointRepository(TransactionsDbContext context)
    {
        _context = context;
    }
    
    public async Task<long> GetCheckpoint(string streamName)
    {
        var checkpoint = await _context.Checkpoints.FirstOrDefaultAsync(x => x.StreamName == streamName);
        return checkpoint?.Position ?? 0;
    }

    public async Task IncrementCheckpoint(string streamName)
    {
        var checkpoint = _context.Checkpoints.First(x => x.StreamName == streamName);
        checkpoint.Position++;
        await _context.SaveChangesAsync();
    }
}