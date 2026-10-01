namespace BankApi.DTOs.ResponseDTOs
{
    public class ClientResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public List<BankAccountResponseDto> Accounts { get; set; } = new();
        public List<PhoneResponseDto> Phones { get; set; } = new();                            
    }
}
