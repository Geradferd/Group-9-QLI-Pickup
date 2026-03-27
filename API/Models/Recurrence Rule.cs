using System.ComponentModel.DataAnnotations;

namespace Api.Models;

/// Work in progress under Gerald Pruitt
public class RecurrenceRule
{
    [Required]
    [MaxLength(100)]
    public string Frequency {get; set;} = string.Empty; /// how often something happens

    [Required]
    public List<string> daysOfWeek {get; set;} = new List<string>(); /// days of the week what something can happen

    [Required]
    [MaxLength(100)]
    public string startDateAndEndDate {get; set;} = string.Empty; /// start and end date of something

    [Required]
    public int maxOccurrences {get; set;} /// number of times something happens
}
