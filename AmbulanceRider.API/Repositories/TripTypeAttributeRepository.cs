using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Repositories;

public class TripTypeAttributeRepository : ITripTypeAttributeRepository
{
    private readonly ApplicationDbContext _context;

    public TripTypeAttributeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TripTypeAttribute>> GetByTripTypeIdAsync(int tripTypeId)
    {
        return await _context.TripTypeAttributes
            .Where(a => a.TripTypeId == tripTypeId && a.DeletedAt == null)
            .OrderBy(a => a.DisplayOrder)
            .ThenBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<TripTypeAttribute?> GetByIdAsync(int id)
    {
        return await _context.TripTypeAttributes
            .Where(a => a.Id == id && a.DeletedAt == null)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(TripTypeAttribute attribute)
    {
        await _context.TripTypeAttributes.AddAsync(attribute);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TripTypeAttribute attribute)
    {
        _context.TripTypeAttributes.Update(attribute);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var attribute = await GetByIdAsync(id);
        if (attribute != null)
        {
            attribute.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(attribute);
        }
    }
}
