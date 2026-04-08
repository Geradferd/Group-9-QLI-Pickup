using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Tables
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Rider> Riders { get; set; } = null!;
    public DbSet<Driver> Drivers { get; set; } = null!;
    public DbSet<Vehicle> Vehicles { get; set; } = null!;
    public DbSet<TransportationType> TransportationTypes { get; set; } = null!;
    public DbSet<Trip> Trips { get; set; } = null!;
    public DbSet<TripStatusHistory> TripStatusHistories { get; set; } = null!;
    public DbSet<GPS_Track_Point> GPS_TrackPoints { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<Hours_of_Operation> OperatingHours { get; set; } = null!;
    public DbSet<SpecialDate> SpecialSchedules { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        // Trip
        modelBuilder.Entity<Trip>()
            .Property(t => t.Status)
            .HasConversion<string>();
        modelBuilder.Entity<Trip>()
            .HasOne(t => t.TransportationType)
            .WithMany(tt => tt.Trips)
            .HasForeignKey(t => t.TransportationTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Trip>()
            .HasOne(t => t.RequestedByUser)
            .WithMany()
            .HasForeignKey(t => t.RequestedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Trip>()
            .HasOne(t => t.ApprovedByUser)
            .WithMany()
            .HasForeignKey(t => t.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // TripStatusHistory
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

        // GPS_Track_Point
        modelBuilder.Entity<GPS_Track_Point>()
            .HasOne(g => g.Driver)
            .WithMany()
            .HasForeignKey(g => g.DriverId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<GPS_Track_Point>()
            .HasOne(g => g.Trip)
            .WithMany(t => t.TrackPoints)
            .HasForeignKey(g => g.TripId)
            .OnDelete(DeleteBehavior.Cascade);
        // Notification
        modelBuilder.Entity<Notification>()
            .Property(n => n.Type)
            .HasConversion<string>();
        modelBuilder.Entity<Notification>()
            .Property(n => n.Channel)
            .HasConversion<string>();
        modelBuilder.Entity<Notification>()
            .Property(n => n.DeliveryStatus)
            .HasConversion<string>();
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.RecipientUser)
            .WithMany()
            .HasForeignKey(n => n.RecipientUserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Trip)
            .WithMany(t => t.Notifications)
            .HasForeignKey(n => n.TripId)
            .OnDelete(DeleteBehavior.SetNull);

        // SpecialDate
        modelBuilder.Entity<SpecialDate>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
