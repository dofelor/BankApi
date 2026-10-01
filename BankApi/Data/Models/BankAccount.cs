namespace BankApi.Data.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "KGS";
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public bool IsClosed { get; set; }
        public ICollection<Card> Cards { get; set; } = new List<Card>();
        


    }
}
