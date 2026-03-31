using System.ComponentModel.DataAnnotations;

namespace Api.Models;

// TransportationType Table in DB
// Configurable categories for trips (e.g., Medical, Personal, Shopping)
// Used for filtering, reporting, and display ordering
public class TransportationType
{
    public int Id { get; set; }

    // Display label
    [Required]
    [MaxLength(100)]
    public string Label { get; set; } = string.Empty;

    // Optional longer description of the type
    [MaxLength(500)]
    public string? Description { get; set; }

    // Determines the order
    public int SortOrder { get; set; } = 0;

    // Active Flag
    public bool IsActive { get; set; } = true;

    // Soft delete
    public bool IsDeleted { get; set; } = false;

    // Navigation Collections

    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}