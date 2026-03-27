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

    public DbSet<User> Users { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<TripStatusHistory> TripStatusHistories { get; set; }

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

        // Trip entities
        modelBuilder.Entity<Trip>()
            .Property(t => t.Status)
            .HasConversion<string>();

        // Trip status history relations and in-database enum storage
        modelBuilder.Entity<TripStatusHistory>()
            .Property(h => h.FromStatus)
            .HasConversion<string>();

        modelBuilder.Entity<TripStatusHistory>()
            .Property(h => h.ToStatus)
            .HasConversion<string>();

        modelBuilder.Entity<TripStatusHistory>()
            .HasOne(h => h.Trip)
            .WithMany(t => t.StatusHistory)
            .HasForeignKey(h => h.TripId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TripStatusHistory>()
            .HasOne(h => h.ChangedByUser)
            .WithMany()
            .HasForeignKey(h => h.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
