using Concertation.Banking.API.Features.Accounts.Entities;
using Concertation.Banking.API.Infrastructure.Database;

namespace Concertation.Banking.API.Infrastructure.Services;

public class BalanceHistoryService
{
    private readonly AppDbContext _db;

    public BalanceHistoryService(AppDbContext db) => _db = db;

    public async Task LogAsync(Guid accountId, decimal oldBalance, decimal newBalance)
    {
        await _db.BalanceHistories.AddAsync(new BalanceHistory
        {
            AccountId = accountId,
            PreviousBalance = oldBalance,
            NewBalance = newBalance
        });

        await _db.SaveChangesAsync();
    }
}
