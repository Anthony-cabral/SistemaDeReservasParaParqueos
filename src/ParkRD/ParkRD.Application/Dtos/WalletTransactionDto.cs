namespace ParkRD.Application.Dtos
{
    public class WalletTransactionDto
    {
        public int Id { get; set; }

        public int WalletId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public int? ReservationId { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
