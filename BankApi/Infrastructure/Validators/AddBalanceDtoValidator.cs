using BankApi.DTOs;
using FluentValidation;

namespace BankApi.Infrastructure.Validators
{
    public class AddBalanceDtoValidator : AbstractValidator<AddBalanceDto>
    {
        public AddBalanceDtoValidator()
        {
            RuleFor(d => d.Amount)
                .GreaterThan(0).WithMessage("The deposit amount must be greater than zero.");
        }
    }
}
