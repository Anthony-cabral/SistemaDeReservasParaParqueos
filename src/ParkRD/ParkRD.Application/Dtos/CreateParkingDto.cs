namespace ParkRD.Application.Dtos
{
    public class CreateParkingDto
    {
        public string Code { get; set; } = string.Empty;

        public decimal HourlyRate { get; set; }

        public decimal DailyRate { get; set; }
    }
}
