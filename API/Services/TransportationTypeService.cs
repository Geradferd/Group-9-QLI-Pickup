using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Models;

namespace Api.Services;

// Handles all Transportation Type business logic
// Communicates with DB to manage transportation types

public class TransportationTypeService
{
    private readonly AppDbContext _context;

    public TransportationTypeService(AppDbContext context)
    {
        _context = context;
    }

    // Get all transportation types (excluding soft-deleted ones by default)
    public async Task<List<TransportationTypeResponse>> GetAllAsync(bool includeDeleted = false)
    {
        var query = _context.TransportationTypes.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(t => !t.IsDeleted);
        }

        var types = await query
            .OrderBy(t => t.SortOrder)
            .ThenBy(t => t.Label)
            .ToListAsync();

        return types.Select(t => MapToResponse(t)).ToList();
    }

    // Get a single transportation type by ID
    public async Task<TransportationTypeResponse?> GetByIdAsync(int id)
    {
        var type = await _context.TransportationTypes
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (type == null)
            return null;

        return MapToResponse(type);
    }

    // Create a new transportation type
    public async Task<TransportationTypeResponse> CreateAsync(CreateTransportationTypeRequest request)
    {
        var transportationType = new TransportationType
        {
            Label = request.Label,
            Description = request.Description,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive,
            IsDeleted = false
        };

        _context.TransportationTypes.Add(transportationType);
        await _context.SaveChangesAsync();

        return MapToResponse(transportationType);
    }

    // Update an existing transportation type
    public async Task<TransportationTypeResponse?> UpdateAsync(int id, UpdateTransportationTypeRequest request)
    {
        var type = await _context.TransportationTypes
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (type == null)
            return null;

        type.Label = request.Label;
        type.Description = request.Description;
        type.SortOrder = request.SortOrder;
        type.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return MapToResponse(type);
    }

    // Soft delete a transportation type
    public async Task<bool> SoftDeleteAsync(int id)
    {
        var type = await _context.TransportationTypes
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (type == null)
            return false;

        // Check if any active trips are using this type
        var hasActiveTrips = await _context.Trips
            .AnyAsync(t => t.TransportationTypeId == id && !t.Status.Equals("Cancelled"));

        if (hasActiveTrips)
        {
            // Don't delete if there are active trips using this type
            return false;
        }

        type.IsDeleted = true;
        type.IsActive = false;

        await _context.SaveChangesAsync();

        return true;
    }

    // Helper method to map entity to response DTO
    private TransportationTypeResponse MapToResponse(TransportationType type)
    {
        return new TransportationTypeResponse
        {
            Id = type.Id,
            Label = type.Label,
            Description = type.Description,
            SortOrder = type.SortOrder,
            IsActive = type.IsActive,
            IsDeleted = type.IsDeleted
        };
    }
}
