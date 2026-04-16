using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

/// DTO for creating a new transportation type
public class CreateTransportationTypeRequest
{
    [Required(ErrorMessage = "Label is required")]
    [MaxLength(100, ErrorMessage = "Label cannot exceed 100 characters")]
    public string Label { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    public int SortOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}

/// DTO for updating an existing transportation type
public class UpdateTransportationTypeRequest
{
    [Required(ErrorMessage = "Label is required")]
    [MaxLength(100, ErrorMessage = "Label cannot exceed 100 characters")]
    public string Label { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    public int SortOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}

/// DTO for transportation type response
public class TransportationTypeResponse
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}
