using System.ComponentModel.DataAnnotations;

namespace BankApi.DTOs.CreateDTOs
{
    public class CreateAccountDto
    {
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "The currency code consists of 3 characters (USD, KGS, EUR)")]
        public string Currency { get; set; } = "KGS";

    }
}
