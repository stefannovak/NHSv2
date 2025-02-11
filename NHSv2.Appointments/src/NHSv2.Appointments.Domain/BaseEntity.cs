using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHSv2.Appointments.Domain;

public class BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private init; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }
    
    [Required]
    public DateTimeOffset UpdatedAt { get; set; }
}