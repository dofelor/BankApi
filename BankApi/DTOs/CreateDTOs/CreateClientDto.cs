using BankApi.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BankApi.DTOs.CreateDTOs
{
    public class CreateClientDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set;  } = string.Empty;

        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "The client must have at least one telephone number")]
        [MinLength(1, ErrorMessage = "Provide at least one phone number")]
        public List<CreatePhoneDto> Phones { get; set; } = new();
    }
}
