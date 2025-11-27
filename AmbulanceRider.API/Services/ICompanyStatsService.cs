using AmbulanceRider.API.DTOs;

namespace AmbulanceRider.API.Services;

public interface ICompanyStatsService
{
    Task<CompanyStatsDto> GetCompanyStatsAsync(int companyId);
    Task<CompanyDashboardDto> GetCompanyDashboardAsync(int companyId);
}
