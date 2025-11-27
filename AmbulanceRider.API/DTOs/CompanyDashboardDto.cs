using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.DTOs;

public class CompanyDashboardDto
{
    public CompanyDto CompanyInfo { get; set; }
    public int UserCount { get; set; }
    public int ActiveTripCount { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int MonthlyTripCount { get; set; }
    public double AverageRating { get; set; }
    public List<TripDashboardDto> RecentTrips { get; set; } = new();
}

public class TripDashboardDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public TripStatus Status { get; set; }
    public decimal? TotalPrice { get; set; }
}
