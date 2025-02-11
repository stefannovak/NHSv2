using System.ComponentModel.DataAnnotations;

namespace NHSv2.Appointments.Domain;

public class EventStoreCheckpoint : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string StreamName { get; set; }

    public long Position { get; set; }
}