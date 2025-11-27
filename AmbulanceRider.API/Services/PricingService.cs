using AmbulanceRider.API.Models;
using AmbulanceRider.API.Repositories;

namespace AmbulanceRider.API.Services;

public class PricingService : IPricingService
{
    private readonly IPricingMatrixRepository _pricingRepo;

    public PricingService(IPricingMatrixRepository pricingRepo)
    {
        _pricingRepo = pricingRepo;
    }

    public async Task<decimal> CalculateTripPrice(decimal weight, decimal height, decimal length, decimal width)
    {
        var matrices = await _pricingRepo.GetByDimensionsAsync(weight, height, length, width);
        return matrices.FirstOrDefault()?.TotalPrice ?? 0;
    }

    public async Task<List<PricingMatrix>> GetApplicablePricingMatrices(decimal weight, decimal height, decimal length, decimal width)
    {
        return (await _pricingRepo.GetByDimensionsAsync(weight, height, length, width)).ToList();
    }
}
