using BankApi.DTOs.CreateDTOs;
using FluentValidation;

namespace BankApi.Infrastructure.Validators
{
    public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
    {
        private static readonly string[] AllowedCurrencies = { "KGS", "USD", "EUR", "RUB", "KZT" };
        public CreateAccountDtoValidator()
        {
            RuleFor(a => a.Currency)
                .NotEmpty().WithMessage("The currency code is a mandatory field.")
                .Length(3).WithMessage("The currency code consists of 3 characters (USD, KGS, EUR)")
                .Must(currency => AllowedCurrencies.Contains(currency.ToUpper()))
                .WithMessage($"Unsupported currency. Allowed values: {string.Join(", ", AllowedCurrencies)}.");
        }
    }
}
