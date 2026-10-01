using System.ComponentModel.DataAnnotations;

namespace BankApi.DTOs.UpdateDTOs
{
    public class UpdateClientDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateOnly BirthDate { get; set; }
    }
}
