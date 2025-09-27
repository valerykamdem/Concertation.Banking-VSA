using Concertation.Banking.API.Features.Users.UpdateUserProfile;
using FluentValidation;

namespace Concertation.Banking.API.Features.Users.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserRequestValidator()
    {
        // ✅ Prénom obligatoire
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Prénom requis");

        // ✅ Nom de famille obligatoire
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Nom requis");

        // ✅ Genre valide – ex: "male", "female", "other"
        RuleFor(x => x.CivilStatus).Must(g =>
                g == "male" ||
                g == "female" ||
                g == "other")
            .WithMessage("Genre invalide – doit être 'male', 'female' ou 'other'");
    }
}


public class UpdateUserConfirmRequestValidator : AbstractValidator<UpdateUserProfileConfirmRequest>
{
    public UpdateUserConfirmRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Pin).NotEmpty().Matches("^[0-9]{4}$"); // Code à 4 chiffres
    }
}
