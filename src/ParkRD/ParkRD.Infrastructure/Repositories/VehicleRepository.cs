using ParkRD.Domain.Entities;
using ParkRD.Infrastructure.Context;
using ParkRD.Infrastructure.Interfaces;

namespace ParkRD.Infrastructure.Repositories
{
    public class VehicleRepository : BaseRepository<Vehicles>, IVehicleRepository
    {
        public VehicleRepository(DataContext context) : base(context)
        {
        }
    }
}
