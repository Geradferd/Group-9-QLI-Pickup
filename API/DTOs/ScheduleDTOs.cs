using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

// ── Operating Hours ──────────────────────────────────────────────

public class OperatingHoursResponse
{
    public int Id { get; set; }
    public string day { get; set; } = string.Empty;
    public string startTime { get; set; } = string.Empty;
    public string endTime { get; set; } = string.Empty;
    public bool enabledFlag { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateOperatingHoursRequest
{
    [Required]
    [MaxLength(100)]
    public string day { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string startTime { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string endTime { get; set; } = string.Empty;

    public bool enabledFlag { get; set; } = true;
    public bool IsActive { get; set; } = true;
}

// ── Special Schedules ────────────────────────────────────────────

public class SpecialScheduleResponse
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string date { get; set; } = string.Empty;
    public string specialStartTime { get; set; } = string.Empty;
    public string specialEndTime { get; set; } = string.Empty;
    public bool closedFlag { get; set; }
    public string description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateSpecialScheduleRequest
{
    public int? UserId { get; set; }

    [MaxLength(100)]
    public string date { get; set; } = string.Empty;

    [MaxLength(100)]
    public string specialStartTime { get; set; } = string.Empty;

    [MaxLength(100)]
    public string specialEndTime { get; set; } = string.Empty;

    public bool closedFlag { get; set; } = false;

    public string description { get; set; } = string.Empty;
}

public class UpdateSpecialDatesRequest
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string date { get; set; } = string.Empty;

    [MaxLength(100)]
    public string specialStartTime { get; set; } = string.Empty;

    [MaxLength(100)]
    public string specialEndTime { get; set; } = string.Empty;

    public bool closedFlag { get; set; } = false;

    public string description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public class UpdateSpecialSchedulesRequest : UpdateSpecialDatesRequest { }
