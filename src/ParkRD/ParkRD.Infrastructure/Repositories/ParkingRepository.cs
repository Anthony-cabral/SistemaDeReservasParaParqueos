using ParkRD.Domain.Entities;
using ParkRD.Infrastructure.Context;
using ParkRD.Infrastructure.Interfaces;

namespace ParkRD.Infrastructure.Repositories
{
    public class ParkingRepository : BaseRepository<Parkings>, IParkingRepository
    {
        public ParkingRepository(DataContext context) : base(context)
        {
        }
    }
}
