using ParkRD.Domain.Core;

namespace ParkRD.Domain.Entities
{
    public class Reservations : BaseEntity
    {
        public int UserId { get; set; }

        public int VehicleId { get; set; }

        public int ParkingId { get; set; }

        public DateTime ReservationDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string ReservationType { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
