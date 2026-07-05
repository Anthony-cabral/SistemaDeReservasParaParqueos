namespace ParkRD.Infrastructure.Models.Dtos
{
    public class ReservationDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public int VehicleId { get; set; }

        public string VehiclePlate { get; set; } = string.Empty;

        public int ParkingId { get; set; }

        public string ParkingCode { get; set; } = string.Empty;

        public DateTime ReservationDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string ReservationType { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
