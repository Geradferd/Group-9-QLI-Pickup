using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Models;

namespace Api.Services;

public class VehicleService
{
    private readonly AppDbContext _context;

    public VehicleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehicleResponse>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Vehicles.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(v => v.IsActive);
        }

        var vehicles = await query
            .OrderBy(v => v.DisplayName)
            .ToListAsync();

        return vehicles.Select(v => MapToResponse(v)).ToList();
    }

    public async Task<VehicleResponse?> GetByIdAsync(int id)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        if (vehicle == null)
            return null;

        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse> CreateAsync(CreateVehicleRequest request)
    {
        var vehicle = new Vehicle
        {
            DisplayName = request.DisplayName,
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            LicensePlate = request.LicensePlate,
            VIN = request.VIN,
            SeatCapacity = request.SeatCapacity,
            WheelchairCapacity = request.WheelchairCapacity,
            Odometer = request.Odometer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse?> UpdateAsync(int id, UpdateVehicleRequest request)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        if (vehicle == null)
            return null;

        vehicle.DisplayName = request.DisplayName;
        vehicle.Make = request.Make;
        vehicle.Model = request.Model;
        vehicle.Year = request.Year;
        vehicle.LicensePlate = request.LicensePlate;
        vehicle.VIN = request.VIN;
        vehicle.SeatCapacity = request.SeatCapacity;
        vehicle.WheelchairCapacity = request.WheelchairCapacity;
        vehicle.Odometer = request.Odometer;

        await _context.SaveChangesAsync();

        return MapToResponse(vehicle);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        if (vehicle == null)
            return false;

        vehicle.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    private VehicleResponse MapToResponse(Vehicle vehicle)
    {
        return new VehicleResponse
        {
            Id = vehicle.Id,
            UserId = vehicle.UserId,
            DisplayName = vehicle.DisplayName,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            LicensePlate = vehicle.LicensePlate,
            VIN = vehicle.VIN,
            SeatCapacity = vehicle.SeatCapacity,
            WheelchairCapacity = vehicle.WheelchairCapacity,
            Odometer = vehicle.Odometer,
            IsActive = vehicle.IsActive,
            CreatedAt = vehicle.CreatedAt
        };
    }
}