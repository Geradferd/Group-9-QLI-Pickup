using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api.Models;

/// data model for the hours that qli pickup is available for
public class Hours_of_Operation
{
    public int Id { get; set; }

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
    public bool enabledFlag { get; set; } = true; /// rather or not the service is open

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;
}