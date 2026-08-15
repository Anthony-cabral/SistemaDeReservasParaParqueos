using ParkRD.Domain.Entities;
using ParkRD.Infrastructure.Context;
using ParkRD.Infrastructure.Interfaces;

namespace ParkRD.Infrastructure.Repositories
{
    public class WalletTransactionRepository : BaseRepository<WalletTransactions>, IWalletTransactionRepository
    {
        public WalletTransactionRepository(DataContext context) : base(context)
        {
        }
    }
}
