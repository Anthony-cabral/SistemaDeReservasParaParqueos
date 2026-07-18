namespace ParkRD.Application.Dtos
{
    public class CreateReservationDto
    {
        public int UserId { get; set; }

        public int VehicleId { get; set; }

        public int ParkingId { get; set; }

        public DateTime ReservationDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string ReservationType { get; set; } = string.Empty;
    }
}
