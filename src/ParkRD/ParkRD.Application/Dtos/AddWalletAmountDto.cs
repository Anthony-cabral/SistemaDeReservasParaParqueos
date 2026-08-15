namespace ParkRD.Application.Dtos
{
    public class AddWalletAmountDto
    {
        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
