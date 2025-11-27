using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;

namespace AmbulanceRider.API.Services;

public class CompanyStatsService : ICompanyStatsService
{
    private readonly IUserRepository _userRepo;
    private readonly ITripRepository _tripRepo;
    private readonly ICompanyRepository _companyRepo;

    public CompanyStatsService(
        IUserRepository userRepo,
        ITripRepository tripRepo,
        ICompanyRepository companyRepo)
    {
        _userRepo = userRepo;
        _tripRepo = tripRepo;
        _companyRepo = companyRepo;
    }

    public async Task<CompanyStatsDto> GetCompanyStatsAsync(int companyId)
    {
        var stats = new CompanyStatsDto
        {
            TotalUsers = (await _userRepo.GetAllAsync()).Count(u => u.CompanyId == companyId),
            ActiveTrips = (await _tripRepo.GetAllAsync())
                .Count(t => t.Status == TripStatus.Approved || t.Status == TripStatus.InProgress),
            CompletedTripsThisMonth = (await _tripRepo.GetAllAsync())
                .Count(t => t.Status == TripStatus.Completed && 
                    t.ActualEndTime.HasValue && 
                    t.ActualEndTime.Value.Month == DateTime.UtcNow.Month),
            RevenueThisMonth = (await _tripRepo.GetAllAsync())
                .Where(t => t.Status == TripStatus.Completed && 
                    t.ActualEndTime.HasValue && 
                    t.ActualEndTime.Value.Month == DateTime.UtcNow.Month)
                .Sum(t => t.TotalPrice ?? 0),
            AverageTripPrice = (await _tripRepo.GetAllAsync())
                .Where(t => t.TotalPrice.HasValue)
                .Average(t => t.TotalPrice ?? 0)
        };

        return stats;
    }

    public async Task<CompanyDashboardDto> GetCompanyDashboardAsync(int companyId)
    {
        // TODO: Implement dashboard logic
        throw new NotImplementedException();
    }
}
