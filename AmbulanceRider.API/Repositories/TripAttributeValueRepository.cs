using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Repositories;

public class TripAttributeValueRepository : ITripAttributeValueRepository
{
    private readonly ApplicationDbContext _context;

    public TripAttributeValueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TripAttributeValue>> GetByTripIdAsync(int tripId)
    {
        return await _context.TripAttributeValues
            .Include(v => v.TripTypeAttribute)
            .Where(v => v.TripId == tripId && v.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<TripAttributeValue?> GetByIdAsync(int id)
    {
        return await _context.TripAttributeValues
            .Where(v => v.Id == id && v.DeletedAt == null)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(TripAttributeValue value)
    {
        await _context.TripAttributeValues.AddAsync(value);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<TripAttributeValue> values)
    {
        await _context.TripAttributeValues.AddRangeAsync(values);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TripAttributeValue value)
    {
        _context.TripAttributeValues.Update(value);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var value = await GetByIdAsync(id);
        if (value != null)
        {
            value.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(value);
        }
    }

    public async Task DeleteByTripIdAsync(int tripId)
    {
        var values = await _context.TripAttributeValues
            .Where(v => v.TripId == tripId && v.DeletedAt == null)
            .ToListAsync();
        
        foreach (var value in values)
        {
            value.DeletedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
    }
}
