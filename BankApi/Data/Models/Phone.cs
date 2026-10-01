using BankApi.Data.Enums;

namespace BankApi.Data.Models
{
    public class Phone
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public PhoneType PhoneType { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
    }
}
