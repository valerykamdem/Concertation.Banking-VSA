using Concertation.Banking.API.Features.Transactions.Requests;
using FluentValidation;

namespace Concertation.Banking.API.Features.Transactions.Validators;

public class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(x => x.FromAccountNumber).NotEmpty().MinimumLength(21);
        RuleFor(x => x.ToAccountNumber).NotEmpty().MinimumLength(21);
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(100_000); // Limite par défaut
    }
}

public class TransactionRequestValidator : AbstractValidator<TransactionRequest>
{
    public TransactionRequestValidator()
    {
        RuleFor(x => x.AccountNumber).NotEmpty().MinimumLength(21);
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(100_000); // Limite par défaut
    }
}

public class TransactionConfirmRequestValidator : AbstractValidator<TransactionConfirmRequest>
{
    public TransactionConfirmRequestValidator()
    {
        RuleFor(x => x.TransactionId).NotEmpty();
        RuleFor(x => x.Pin).NotEmpty().Matches("^[0-9]{4}$"); // Code à 4 chiffres
    }
}
