using Concertation.Banking.API.Features.Accounts.Entities;
using Concertation.Banking.API.Features.Users.Entities;
using Concertation.Banking.API.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Concertation.Banking.API.Infrastructure.Services;

public class ValidateService
{
    private readonly AppDbContext _db;

    public ValidateService(AppDbContext db) => _db = db;

    public async Task<bool> ValidateOwnership(string accountNumber, string userId)
    {
        Account? account = await _db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account is null)
            return false;

        // Convert userId to Guid for comparison
        if (!Guid.TryParse(userId, out Guid userGuid))
            return false;

        return account.UserId == userGuid;
    }

    public async Task<bool> ValidateLinkedAccounts(string fromAccountNumber, string toAccountNumber)
    {
        Account? fromAccount = await _db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == fromAccountNumber);
        Account? toAccount = await _db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == toAccountNumber);

        if (fromAccount is null || toAccount is null)
            return false;

        // Un compte épargne peut-il envoyer vers un compte non lié ?
        if (fromAccount.Type == AccountType.Savings && fromAccount.LinkedAccountId != toAccount.Id)
            return false;

        return true;
    }

    public async Task<bool> ValidateSufficientBalance(string accountNumber, decimal amount)
    {
        Account? account = await _db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        return account?.Balance >= amount;
    }

    public async Task<bool> ValidatePinAsync(string userId, string pin)
    {
        User? user = await _db.Users.FindAsync(Guid.Parse(userId));
        return user?.PinHash != null && BCrypt.Net.BCrypt.Verify(pin, user.PinHash);
    }
}
