using BankApi.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankApi.DTOs.CreateDTOs
{
    public class CreatePhoneDto
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public PhoneType PhoneType { get; set; }

    }
}
