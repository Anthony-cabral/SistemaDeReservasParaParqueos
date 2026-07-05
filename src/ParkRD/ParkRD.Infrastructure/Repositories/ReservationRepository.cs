using ParkRD.Domain.Entities;
using ParkRD.Infrastructure.Context;
using ParkRD.Infrastructure.Interfaces;

namespace ParkRD.Infrastructure.Repositories
{
    public class ReservationRepository : BaseRepository<Reservations>, IReservationRepository
    {
        public ReservationRepository(DataContext context) : base(context)
        {
        }
    }
}
