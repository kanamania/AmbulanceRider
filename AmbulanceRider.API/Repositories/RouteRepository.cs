using AmbulanceRider.API.Data;

namespace AmbulanceRider.API.Repositories;

public class RouteRepository : Repository<Models.Route>, IRouteRepository
{
    public RouteRepository(ApplicationDbContext context) : base(context)
    {
    }
}
