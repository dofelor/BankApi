using BankApi.DTOs.CreateDTOs;
using FluentValidation;

namespace BankApi.Infrastructure.Validators
{
    public class CreatePhoneDtoValidator : AbstractValidator<CreatePhoneDto>
    {
        public CreatePhoneDtoValidator()
        {
            RuleFor(p => p.PhoneNumber)
                .NotEmpty().WithMessage("The phone number is a required field.")
                .Matches(@"^\+996\d{9}$")
                .WithMessage("The phone number must be in the Kyrgyzstan format (+996XXXXXXXXX, 12 digits after the +).");

            RuleFor(p => p.PhoneType)
                .IsInEnum().WithMessage("An invalid phone type has been specified. Valid values: Mobile (0), Home (1), Work (2)");
        }
    }
}
