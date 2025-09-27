using Concertation.Banking.API.Features.Accounts.Entities;

namespace Concertation.Banking.API.Features.Transactions.Entities;

public enum TransactionType
{
    InternalTransfer,
    //ExternalTransfer,
    Deposit,
    Withdrawal
}

public enum TransactionStatus
{
    Completed,
    Pending,
    Failed
}

public class Transaction
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string TransferId { get; set; } = string.Empty;

    // 👇 Pour transfert interne/externe
    public Guid? FromAccountId { get; set; }
    public Guid? ToAccountId { get; set; }

    // 👇 Pour dépôt/retrait
    public Guid? AccountId { get; set; }

    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // 👇 Navigation props
    public Account? FromAccount { get; set; }
    public Account? ToAccount { get; set; }
    public Account? Account { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Completed;
}
