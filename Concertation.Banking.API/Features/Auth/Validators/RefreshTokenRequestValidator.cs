using Concertation.Banking.API.Features.Auth.Requests;
using FluentValidation;

namespace Concertation.Banking.API.Features.Auth.Validators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
