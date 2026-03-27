using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Api.Models;

public enum TripStatus
{
    Requested,
    Assigned,
    DriverEnRoute,
    DriverArrived,
    InProgress,
    Completed,
    Cancelled
}

public class Trip
{
    public int Id { get; set; }

    public int? RiderId { get; set; }
    public Rider? Rider { get; set; }

    public int? DriverId { get; set; }
    public Driver? Driver { get; set; }

    [Required]
    public TripStatus Status { get; set; } = TripStatus.Requested;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TripStatusHistory> StatusHistory { get; set; } = new List<TripStatusHistory>();
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
