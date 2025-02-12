using Microsoft.EntityFrameworkCore;
using NHSv2.Appointments.Domain;
using NHSv2.Appointments.Domain.Appointments;

namespace NHSv2.Appointments.Infrastructure.Data;

public class AppointmentsDbContext : DbContext
{
    public AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Appointment> Appointments { get; set; }
    
    public DbSet<EventStoreCheckpoint> EventStoreCheckpoints { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>().HasKey(x => x.Id);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var time = DateTimeOffset.UtcNow;
        var createdEntities = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added)
            .Select(x => x.Entity)
            .OfType<BaseEntity>()
            .ToList();
        
        foreach (var entity in createdEntities)
        {
            entity.CreatedAt = time;
            entity.UpdatedAt = time;
        }
        
        var updatedEntities = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Modified)
            .Select(x => x.Entity)
            .OfType<BaseEntity>()
            .ToList();
        
        foreach (var entity in updatedEntities)
        {
            entity.UpdatedAt = time;
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}