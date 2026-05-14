using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Helpers;

/// <summary>
/// Creates a fresh in-memory EF Core database for each test so tests are fully isolated.
/// </summary>
public static class TestDbFactory
{
    public static AppDbContext CreateInMemory(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        var ctx = new AppDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    /// <summary>
    /// Seeds the minimum required rows so FK constraints don't block trip creation.
    /// Returns (adminUserId, transportationTypeId).
    /// </summary>
    public static (int adminId, int ttId) SeedDefaults(AppDbContext ctx)
    {
        var admin = new User
        {
            Email = "admin@qli.test",
            PasswordHash = "hashed",
            DisplayName = "Admin",
            Role = UserRole.Admin,
            IsActive = true
        };
        ctx.Users.Add(admin);

        var tt = new TransportationType { Label = "Van", Description = "Standard van" };
        ctx.TransportationTypes.Add(tt);

        ctx.SaveChanges();
        return (admin.Id, tt.Id);
    }
}
