using Concertation.Banking.API.Features.Transactions.Entities;
using Concertation.Banking.API.Infrastructure.Database;

namespace Concertation.Banking.API.Features.Transactions.Handlers;

public class TransactionFactory
{
    private readonly AppDbContext _db;

    public TransactionFactory(AppDbContext db) => _db = db;

    public async Task CreateAsync(
        Guid? fromAccountId,
        Guid? toAccountId,
        Guid? accountId,
        decimal amount,
        string transferId,
        string? reason,
        TransactionType type)
    {
        var transaction = new Transaction
        {
            TransferId = transferId,
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            AccountId = accountId,
            Amount = amount,
            Reason = reason,
            Type = type,
            Timestamp = DateTime.UtcNow
        };

        await _db.Transactions.AddAsync(transaction);
        await _db.SaveChangesAsync();
    }
}
