using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Repositories;

public class TripTypeRepository : ITripTypeRepository
{
    private readonly ApplicationDbContext _context;

    public TripTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TripType>> GetAllAsync()
    {
        return await _context.TripTypes
            .Where(tt => tt.DeletedAt == null)
            .OrderBy(tt => tt.DisplayOrder)
            .ThenBy(tt => tt.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<TripType>> GetActiveAsync()
    {
        return await _context.TripTypes
            .Where(tt => tt.DeletedAt == null && tt.IsActive)
            .OrderBy(tt => tt.DisplayOrder)
            .ThenBy(tt => tt.Name)
            .ToListAsync();
    }

    public async Task<TripType?> GetByIdAsync(int id)
    {
        return await _context.TripTypes
            .Where(tt => tt.Id == id && tt.DeletedAt == null)
            .FirstOrDefaultAsync();
    }

    public async Task<TripType?> GetByIdWithAttributesAsync(int id)
    {
        return await _context.TripTypes
            .Include(tt => tt.Attributes.Where(a => a.DeletedAt == null))
            .Where(tt => tt.Id == id && tt.DeletedAt == null)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(TripType tripType)
    {
        await _context.TripTypes.AddAsync(tripType);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TripType tripType)
    {
        _context.TripTypes.Update(tripType);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var tripType = await GetByIdAsync(id);
        if (tripType != null)
        {
            tripType.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(tripType);
        }
    }
}
