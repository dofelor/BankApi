namespace BankApi.Services.Generators
{
    public class AccountNumberGenerator : IAccountNumberGenerator
    {
        private static readonly Random _random = new();
        public string Generate()
        {
            var suffix = string.Concat(Enumerable.Range(0, 11).Select(_ => _random.Next(0, 10).ToString()));
            return $"128001{suffix}";

        }
    }
}
