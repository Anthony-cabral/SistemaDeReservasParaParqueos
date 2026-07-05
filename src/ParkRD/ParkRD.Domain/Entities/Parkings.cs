using ParkRD.Domain.Core;

namespace ParkRD.Domain.Entities
{
    public class Parkings : BaseEntity
    {
        public string Code { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal HourlyRate { get; set; }

        public decimal DailyRate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
