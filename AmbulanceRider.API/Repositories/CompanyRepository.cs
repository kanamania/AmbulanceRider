using AmbulanceRider.API.Data;
using AmbulanceRider.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceRider.API.Repositories;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    public CompanyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Company?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
    }

    public override async Task<IEnumerable<Company>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.Users)
            .Where(c => c.DeletedAt == null)
            .ToListAsync();
    }
}
