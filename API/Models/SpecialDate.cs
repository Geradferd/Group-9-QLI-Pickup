using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api.Models;

/// data model for the hours that qli pickup is available for
public class SpecialDate
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