namespace AmbulanceRider.API.DTOs;

public class CompanyStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveTrips { get; set; }
    public int CompletedTripsThisMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal AverageTripPrice { get; set; }
}
