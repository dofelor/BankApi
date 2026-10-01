namespace BankApi.DTOs.ResponseDTOs
{
    public class BankAccountResponseDto
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "KGS";
        public bool IsClosed { get; set; }
        public int ClientId { get; set; }
        public List<CardResponseDto> Cards { get; set; } = new();
    }
}
