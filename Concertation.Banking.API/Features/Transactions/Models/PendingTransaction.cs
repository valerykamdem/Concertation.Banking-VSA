using Concertation.Banking.API.Features.Transactions.Entities;

namespace Concertation.Banking.API.Features.Transactions.Models;

public class PendingTransaction
{
    public string TransferId { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public TransactionType Type { get; set; }
    public string? FromAccountNumber { get; set; }
    public string? ToAccountNumber { get; set; }
    public string? AccountNumber { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
