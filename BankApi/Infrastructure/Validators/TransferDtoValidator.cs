using BankApi.DTOs;
using FluentValidation;

namespace BankApi.Infrastructure.Validators
{
    public class TransferDtoValidator : AbstractValidator<TransferDto>
    {
        public TransferDtoValidator()
        {
            RuleFor(t => t.FromAccountId)
                .GreaterThan(0).WithMessage("Enter the correct sender account ID.");

            RuleFor(t => t.ToAccountId)
                .GreaterThan(0).WithMessage("Enter the correct recipient account ID.")
                .NotEqual(t => t.FromAccountId).WithMessage("Funds cannot be transferred to the same account.");

            RuleFor(t => t.Amount)
                .GreaterThan(0).WithMessage("The transfer amount must be greater than zero.");
        }
    }
}
