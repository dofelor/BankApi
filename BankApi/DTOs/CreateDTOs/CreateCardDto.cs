using BankApi.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace BankApi.DTOs.CreateDTOs
{
    public class CreateCardDto
    {
        [Required]
        public CardType CardType { get; set; }

    }
}
