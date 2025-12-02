using AmbulanceRider.API.DTOs;

namespace AmbulanceRider.API.Services;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDto>> GetAllAsync();
    Task<CompanyDto?> GetByIdAsync(int id);
    Task<CompanyDto> CreateAsync(CreateCompanyDto dto, Guid createdBy);
    Task<CompanyDto> UpdateAsync(int id, UpdateCompanyDto dto, Guid updatedBy);
    Task DeleteAsync(int id, Guid deletedBy);
}
