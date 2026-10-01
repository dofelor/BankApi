using BankApi.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankApi.DTOs.CreateDTOs
{
    public class CreatePhoneDto
    {
        public string PhoneNumber { get; set; } = string.Empty;

        public PhoneType PhoneType { get; set; }

    }
}
