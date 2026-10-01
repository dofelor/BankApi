namespace BankApi.Services.Generators
{
    public class CardNumberGenerator : ICardNumberGenerator
    {
        private static readonly Random _random = new();
        public string GenerateCardNumber()
        {
            var body =string.Concat(Enumerable.Range(0, 12).Select(_ => _random.Next(0, 10).ToString()));
            return $"4192{body}";
        }

        public DateOnly GenerateValidityPeriod(int yearsValid = 5)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return today.AddYears(yearsValid);
        }

    }
}
