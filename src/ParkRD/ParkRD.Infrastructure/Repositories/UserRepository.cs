using ParkRD.Domain.Entities;
using ParkRD.Infrastructure.Context;
using ParkRD.Infrastructure.Interfaces;

namespace ParkRD.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<Users>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }
    }
}
