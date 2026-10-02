namespace BankApi.Data.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName {  get; set; } = string.Empty;
        public string? MiddleName { get; set;  }
        public string Email { get; set; } = string.Empty;
        public DateOnly BirthDate{ get; set; }
        public string FullName => string.IsNullOrWhiteSpace(MiddleName)
            ? $"{LastName} {FirstName}".Trim()
            : $"{LastName} {FirstName} {MiddleName}".Trim();

        public ICollection<Phone> PhoneNumbers { get; set; } = new List<Phone>();
        public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

    }
}
