using ParkRD.Domain.Core;

namespace ParkRD.Domain.Entities
{
    public class Vehicles : BaseEntity
    {
        public string Plate { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int UserId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
