namespace ParkRD.API.Models.Dtos
{
    public class UpdateVehicleDto
    {
        public int Id { get; set; }

        public string Plate { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int UserId { get; set; }

        public bool IsActive { get; set; }
    }
}
