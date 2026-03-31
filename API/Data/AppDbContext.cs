using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data;

//class for database and C# code to connect
//Handles all communication

public class AppDbContext : DbContext
{
    //constructor to receive database settings
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    //represents table in database

    public DbSet<Trip> Trips { get; set; } = null!;
    public DbSet<TransportationType> TransportationTypes { get; set; } = null!;
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating (ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //no two users can have same email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        //stores the roles inside the DB
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();
        
        // Trip/TransportationType Relationship
        modelBuilder.Entity<Trip>()
            .HasOne(t => t.TransportationType)
            .WithMany(tt => tt.Trips)
            .HasForeignKey(t => t.TransportationTypeId)
            .OnDelete(DeleteBehavior.Restrict);

    }

}
