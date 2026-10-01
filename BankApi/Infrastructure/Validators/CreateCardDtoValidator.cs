using BankApi.DTOs.CreateDTOs;
using FluentValidation;

namespace BankApi.Infrastructure.Validators
{
    public class CreateCardDtoValidator : AbstractValidator<CreateCardDto>
    {
        public CreateCardDtoValidator()
        {
            RuleFor(c => c.CardType)
                .IsInEnum().WithMessage("Invalid card type specified. Valid values: Debit (0), Credit (1).");
        }
    }
}
