using System.ComponentModel.DataAnnotations;

namespace NHSv2.Payments.Domain;

public class PaymentsEventStoreCheckpoint : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string StreamName { get; init; } = "payments";

    public long Position { get; set; }
}