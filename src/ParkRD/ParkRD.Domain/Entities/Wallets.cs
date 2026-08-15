using ParkRD.Domain.Core;

namespace ParkRD.Domain.Entities
{
    public class Wallets : BaseEntity
    {
        public int UserId { get; set; }

        public decimal Balance { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
