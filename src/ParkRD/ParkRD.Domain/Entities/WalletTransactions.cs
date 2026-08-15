using ParkRD.Domain.Core;

namespace ParkRD.Domain.Entities
{
    public class WalletTransactions : BaseEntity
    {
        public int WalletId { get; set; }

        public int UserId { get; set; }

        public int? ReservationId { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
