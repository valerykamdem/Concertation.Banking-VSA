using Concertation.Banking.API.Features.Users.Entities;

namespace Concertation.Banking.API.Features.Accounts.Entities;

public enum AccountType
{
    Checking,
    Savings
}

public class Account
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; }
    public decimal Balance { get; set; } = 0.0m;
    public AccountType Type { get; set; }
    public string AccountNumber { get; set; } = string.Empty;

    // Si c'est un compte épargne, il pointe vers un compte courant
    public Guid? LinkedAccountId { get; set; }
    public Account? LinkedAccount { get; set; }

    // 👇 Ajoute cette navigation property
    //public string UserIdentityId { get; set; } = string.Empty;
    public User? User { get; set; }
}
