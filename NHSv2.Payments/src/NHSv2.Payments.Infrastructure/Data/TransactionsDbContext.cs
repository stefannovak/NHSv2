using Microsoft.EntityFrameworkCore;
using NHSv2.Payments.Domain;
using NHSv2.Payments.Domain.Transactions;

namespace NHSv2.Payments.Infrastructure.Data;

public class TransactionsDbContext : DbContext
{
    public TransactionsDbContext(DbContextOptions<TransactionsDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Payment> Payments { get; set; }

    public DbSet<PaymentsEventStoreCheckpoint> Checkpoints { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>().HasKey(x => x.Id);
        
        modelBuilder.Entity<Payment>()
            .Property(p => p.Status)
            .HasConversion<string>();  // 
        
        modelBuilder.Entity<Payment>()
            .Property(p => p.Type)
            .HasConversion<string>();  // 
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