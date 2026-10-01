using BankApi.DTOs.UpdateDTOs;
using FluentValidation;

namespace BankApi.Infrastructure.Validators
{
    public class UpdateClientDtoValidator : AbstractValidator<UpdateClientDto>
    {
        public UpdateClientDtoValidator()
        {
            RuleFor(c => c.FirstName)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("The name must not exceed 50 characters.")
                .Matches(@"^[a-zA-Zа-яА-ЯКыргызстанӨөҮүҢң\s-]+$")
                .WithMessage("The name may contain only letters, spaces, and hyphens.");

            RuleFor(c => c.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("The last name must not exceed 50 characters.")
                .Matches(@"^[a-zA-Zа-яА-ЯКыргызстанӨөҮүҢң\s-]+$")
                .WithMessage("The last name may contain only letters, spaces, and hyphens.");

            RuleFor(c => c.MiddleName)
                .MaximumLength(50).WithMessage("The middle name must not exceed 50 characters.")
                .Matches(@"^[a-zA-Zа-яА-ЯКыргызстанӨөҮүҢң\s-]+$")
                .WithMessage("The middle name may contain only letters, spaces, and hyphens.")
                .When(c => !string.IsNullOrEmpty(c.MiddleName));

            RuleFor(c => c.BirthDate)
                .NotEmpty().WithMessage("Birth date is required.")
                .Must(BeAtLeast18YearsOld)
                .WithMessage("The client must be of legal age (over 18 years old).")
                .Must(BeValidAge)
                .WithMessage("An incorrect date of birth has been entered.");

        }


        private static bool BeAtLeast18YearsOld(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return birthDate <= today.AddYears(-18);
        }

        private static bool BeValidAge(DateOnly birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return birthDate > today.AddYears(-120);
        }
    }
}
