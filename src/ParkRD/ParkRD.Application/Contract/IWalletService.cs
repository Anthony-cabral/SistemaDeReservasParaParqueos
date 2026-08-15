using ParkRD.Application.Core;
using ParkRD.Application.Dtos;

namespace ParkRD.Application.Contract
{
    public interface IWalletService
    {
        ServiceResult<IEnumerable<WalletDto>> GetAll();

        ServiceResult<WalletDto> GetByUser(int userId);

        ServiceResult<IEnumerable<WalletTransactionDto>> GetTransactions();

        ServiceResult<IEnumerable<WalletTransactionDto>> GetTransactionsByUser(int userId);

        ServiceResult<bool> AddAmount(AddWalletAmountDto request);
    }
}
