using System.ComponentModel.DataAnnotations;
namespace Api.Models;

//Rider Table in DB

public class Rider
{
    public int Id {get; set;}
    [Required]
    public int UserId {get; set;} //foreign key
    public User User {get; set;} = null;

    [Required]
    [MaxLength(100)]
    public string FirstName {get; set;} = string.Empty;

    [MaxLength(20)]
    public string? Phone {get; set;} //optional

    [MaxLength(20)]
    public string? RoomNumber {get; set;} //QLI room number

    [MaxLength(500)]
    public string? MobilityNotes {get; set;} //wheelchair etc

    [MaxLength(100)]
    public string? EmergencyContactName {get; set;}

    [MaxLength(20)]
    public string? EmergencyContactPhone {get; set;}

    public bool IsActive {get; set;} = true; //soft delete

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;    



}   