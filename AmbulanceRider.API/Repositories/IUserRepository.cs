using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdWithRolesAsync(int id);
    Task<IEnumerable<User>> GetAllWithRolesAsync();
}
