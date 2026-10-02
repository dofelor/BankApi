using BankApi.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BankApi.DTOs.CreateDTOs
{
    public class CreateClientDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set;  } = string.Empty;

        public string? MiddleName { get; set; }

        public string Email { get; set; } = string.Empty;

        public DateOnly BirthDate { get; set; }

        public List<CreatePhoneDto> PhoneNumbers { get; set; } = new();
    }
}
