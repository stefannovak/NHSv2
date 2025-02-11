using Microsoft.EntityFrameworkCore;
using NHSv2.Appointments.Domain.Appointments;

namespace NHSv2.Appointments.Infrastructure.Data;

public class AppointmentsDbContext : DbContext
{
    public AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Appointment> Appointments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>().HasKey(x => x.Id);
    }
}