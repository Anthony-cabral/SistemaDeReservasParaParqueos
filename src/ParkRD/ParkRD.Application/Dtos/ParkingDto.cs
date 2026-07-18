namespace ParkRD.Application.Dtos
{
    public class ParkingDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal HourlyRate { get; set; }

        public decimal DailyRate { get; set; }

        public bool IsActive { get; set; }
    }
}
