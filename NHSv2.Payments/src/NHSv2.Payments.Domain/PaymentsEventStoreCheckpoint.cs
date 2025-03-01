using System.ComponentModel.DataAnnotations;

namespace NHSv2.Payments.Domain;

public class PaymentsEventStoreCheckpoint : BaseEntity
{
    // EF Core requires a parameterless constructor
    public PaymentsEventStoreCheckpoint()
    {
    }
    
    public PaymentsEventStoreCheckpoint(string streamName)
    {
        StreamName = streamName;
    }
    
    [Required]
    [MaxLength(100)]
    public string StreamName { get; private set; }

    public long Position { get; set; }
}