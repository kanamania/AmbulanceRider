using AmbulanceRider.API.Models;

namespace AmbulanceRider.API.Services;

public interface IPricingService
{
    Task<decimal> CalculateTripPrice(decimal weight, decimal height, decimal length, decimal width);
    Task<List<PricingMatrix>> GetApplicablePricingMatrices(decimal weight, decimal height, decimal length, decimal width);
}
