using ParkRD.Application.Contract;
using ParkRD.Application.Core;
using ParkRD.Application.Dtos;
using ParkRD.Domain.Entities;
using ParkRD.Infrastructure.Interfaces;

namespace ParkRD.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IUserRepository _userRepository;

        public WalletService(IWalletRepository walletRepository, IWalletTransactionRepository walletTransactionRepository, IUserRepository userRepository)
        {
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _userRepository = userRepository;
        }

        public ServiceResult<IEnumerable<WalletDto>> GetAll()
        {
            var wallets = _walletRepository.GetAll()
                .Select(wallet => MapWallet(wallet))
                .ToList();

            return new ServiceResult<IEnumerable<WalletDto>>
            {
                Success = true,
                Data = wallets
            };
        }

        public ServiceResult<WalletDto> GetByUser(int userId)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
            {
                return new ServiceResult<WalletDto>
                {
                    Success = false,
                    Message = "The selected user was not found."
                };
            }

            var wallet = GetOrCreateWallet(userId);

            return new ServiceResult<WalletDto>
            {
                Success = true,
                Data = MapWallet(wallet)
            };
        }

        public ServiceResult<IEnumerable<WalletTransactionDto>> GetTransactions()
        {
            var transactions = _walletTransactionRepository.GetAll()
                .OrderByDescending(transaction => transaction.CreatedAt)
                .Select(transaction => MapTransaction(transaction))
                .ToList();

            return new ServiceResult<IEnumerable<WalletTransactionDto>>
            {
                Success = true,
                Data = transactions
            };
        }

        public ServiceResult<IEnumerable<WalletTransactionDto>> GetTransactionsByUser(int userId)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
            {
                return new ServiceResult<IEnumerable<WalletTransactionDto>>
                {
                    Success = false,
                    Message = "The selected user was not found."
                };
            }

            var transactions = _walletTransactionRepository.GetAll()
                .Where(transaction => transaction.UserId == userId)
                .OrderByDescending(transaction => transaction.CreatedAt)
                .Select(transaction => MapTransaction(transaction))
                .ToList();

            return new ServiceResult<IEnumerable<WalletTransactionDto>>
            {
                Success = true,
                Data = transactions
            };
        }

        public ServiceResult<bool> AddAmount(AddWalletAmountDto request)
        {
            if (request.UserId <= 0)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "You need to select a user."
                };
            }

            if (request.Amount <= 0)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The amount must be greater than zero."
                };
            }

            var user = _userRepository.GetById(request.UserId);

            if (user == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected user was not found."
                };
            }

            var wallet = GetOrCreateWallet(request.UserId);

            wallet.Balance += request.Amount;
            wallet.UpdatedAt = DateTime.Now;

            var transaction = new WalletTransactions
            {
                WalletId = wallet.Id,
                UserId = request.UserId,
                TransactionType = "Recharge",
                Amount = request.Amount,
                Description = string.IsNullOrWhiteSpace(request.Description) ? "Wallet recharge" : request.Description,
                Status = "Completed",
                CreatedAt = DateTime.Now
            };

            _walletRepository.Update(wallet);
            _walletTransactionRepository.Create(transaction);
            _walletRepository.SaveChanges();

            return new ServiceResult<bool>
            {
                Success = true,
                Data = true
            };
        }

        private Wallets GetOrCreateWallet(int userId)
        {
            var wallet = _walletRepository.GetAll()
                .FirstOrDefault(wallet => wallet.UserId == userId);

            if (wallet != null)
            {
                return wallet;
            }

            wallet = new Wallets
            {
                UserId = userId,
                Balance = 0,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _walletRepository.Create(wallet);
            _walletRepository.SaveChanges();

            return wallet;
        }

        private WalletDto MapWallet(Wallets wallet)
        {
            var user = _userRepository.GetById(wallet.UserId);

            return new WalletDto
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                UserName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}",
                Balance = wallet.Balance,
                IsActive = wallet.IsActive,
                CreatedAt = wallet.CreatedAt,
                UpdatedAt = wallet.UpdatedAt
            };
        }

        private WalletTransactionDto MapTransaction(WalletTransactions transaction)
        {
            var user = _userRepository.GetById(transaction.UserId);

            return new WalletTransactionDto
            {
                Id = transaction.Id,
                WalletId = transaction.WalletId,
                UserId = transaction.UserId,
                UserName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}",
                ReservationId = transaction.ReservationId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Status = transaction.Status,
                CreatedAt = transaction.CreatedAt
            };
        }
    }
}
