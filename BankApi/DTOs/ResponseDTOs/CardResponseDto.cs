using BankApi.Data.Enums;

namespace BankApi.DTOs.ResponseDTOs
{
    public class CardResponseDto
    {
        public int Id { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public DateOnly ValidityPeriod { get; set; }
        public CardType CardType { get; set; }
        public int BankAccountId { get; set; }
        public bool IsBlocked { get; set; }
    }
}
