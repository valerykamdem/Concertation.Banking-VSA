using Concertation.Banking.API.Features.Users.Requests;
using FluentValidation;

namespace Concertation.Banking.API.Features.Users.Validators;

public class SetPinRequestValidator : AbstractValidator<SetPinRequest>
{
    public SetPinRequestValidator()
    {
        RuleFor(x => x.Pin).NotEmpty().Matches("^[0-9]{4}$"); // Code à 4 chiffres
    }
}
