using Concertation.Banking.API.Features.Accounts.Entities;

namespace Concertation.Banking.API.Features.Users.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string IdentityId { get; set; } = string.Empty;
    public string CivilStatus { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;   
    public string Email { get; set; } = string.Empty;
    public string PinHash { get; set; } = string.Empty;

    public ICollection<UserRole> Roles { get; set; } = [];
    public ICollection<Account> Accounts { get; set; } = []; // Navigation property to accounts
}
