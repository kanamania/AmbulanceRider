using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface ICompanyRepository : IRepository<Company>
{
    new Task<Company?> GetByIdAsync(int id);
    new Task<IEnumerable<Company>> GetAllAsync();
}
