namespace ParkRD.API.Models.Dtos
{
    public class CreateVehicleDto
    {
        public string Plate { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int UserId { get; set; }
    }
}
