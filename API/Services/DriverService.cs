using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Models;

namespace Api.Services;

public class DriverService
{
    private readonly AppDbContext _context;

    public DriverService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DriverResponse>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Drivers.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(d => d.IsActive);
        }

        var drivers = await query
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();

        return drivers.Select(d => MapToResponse(d)).ToList();
    }

    public async Task<DriverResponse?> GetByIdAsync(int id)
    {
        var driver = await _context.Drivers
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

        if (driver == null)
            return null;

        return MapToResponse(driver);
    }

    public async Task<DriverResponse?> CreateAsync(CreateDriverRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
            return null;

        var driver = new Driver
        {
            UserId = request.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            LicenseNumber = request.LicenseNumber,
            LicenseExpiry = request.LicenseExpiry,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        return MapToResponse(driver);
    }

    public async Task<DriverResponse?> UpdateAsync(int id, UpdateDriverRequest request)
    {
        var driver = await _context.Drivers
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

        if (driver == null)
            return null;

        driver.FirstName = request.FirstName;
        driver.LastName = request.LastName;
        driver.Phone = request.Phone;
        driver.LicenseNumber = request.LicenseNumber;
        driver.LicenseExpiry = request.LicenseExpiry;

        await _context.SaveChangesAsync();

        return MapToResponse(driver);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var driver = await _context.Drivers
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

        if (driver == null)
            return false;

        driver.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    private DriverResponse MapToResponse(Driver driver)
    {
        return new DriverResponse
        {
            Id = driver.Id,
            UserId = driver.UserId,
            FirstName = driver.FirstName,
            LastName = driver.LastName,
            Phone = driver.Phone,
            LicenseNumber = driver.LicenseNumber,
            LicenseExpiry = driver.LicenseExpiry,
            IsActive = driver.IsActive,
            CreatedAt = driver.CreatedAt
        };
    }
}