using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;


// define what data gets sent to and from the API
// We use these instead of the User model 
// dont expose sensitive data like the password hash

/// DTO reading and updating for the operating hours 
public class OperatingHoursTypeRequest
{
    [Required]
    [MaxLength(100)]
    public string day { get; set; } = string.Empty; /// the current day of the week

    [Required]
    [MaxLength(100)]
    public string startTime { get; set; } = string.Empty; /// time that hours of operation starts

    [Required]
    [MaxLength(100)]
    public string endTime { get; set; } = string.Empty; /// time that hours of operation ends

    [Required]
    public bool enabledFlag = true; /// rather or not the service is open
}

/// DTO CRUD for special dates
public class SpecialDatesTypeRequest
{
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string date { get; set; } = string.Empty; /// date for a activity

    [MaxLength(100)]
    public string specialStartTime { get; set; } = string.Empty; /// time that hours of operation starts

    [MaxLength(100)]
    public string specialEndTime { get; set; } = string.Empty; /// time that hours of operation ends

    public bool closedFlag { get; set; } = false; /// rather or not the activity if available

    public string description { get; set; } = string.Empty; /// descirption for the special date
}
