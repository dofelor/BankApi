namespace BankApi.Services.Generators
{
    public interface ICardNumberGenerator
    {
        string GenerateCardNumber();
        DateOnly GenerateValidityPeriod(int yearsValid = 5);
    }
}
