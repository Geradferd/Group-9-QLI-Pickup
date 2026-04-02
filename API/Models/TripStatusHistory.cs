using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Api.Models;

public enum TripStatus
{
    Pending,
    Approved,
    Denied,
    Scheduled,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}

[Index(nameof(TripId), nameof(ChangedAt))]
public class TripStatusHistory
{
    public int Id { get; set; }

    [Required]
    public int TripId { get; set; }
    public Trip Trip { get; set; } = null!;

    [Required]
    public TripStatus FromStatus { get; set; }

    [Required]
    public TripStatus ToStatus { get; set; }

    [Required]
    public int ChangedByUserId { get; set; }
    public User ChangedByUser { get; set; } = null!;

    [MaxLength(1000)]
    public string? Reason { get; set; }

    [Required]
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
