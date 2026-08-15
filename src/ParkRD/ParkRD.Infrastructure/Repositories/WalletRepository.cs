using ParkRD.Domain.Entities;
using ParkRD.Infrastructure.Context;
using ParkRD.Infrastructure.Interfaces;

namespace ParkRD.Infrastructure.Repositories
{
    public class WalletRepository : BaseRepository<Wallets>, IWalletRepository
    {
        public WalletRepository(DataContext context) : base(context)
        {
        }
    }
}
