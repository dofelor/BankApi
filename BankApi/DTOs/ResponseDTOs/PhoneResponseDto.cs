using BankApi.Data.Enums;

namespace BankApi.DTOs.ResponseDTOs
{
    public class PhoneResponseDto
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public PhoneType PhoneType { get; set; }
        public int ClientId { get; set; }
    }
}
