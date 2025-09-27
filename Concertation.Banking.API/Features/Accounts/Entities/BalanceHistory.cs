namespace Concertation.Banking.API.Features.Accounts.Entities;

public class BalanceHistory
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid AccountId { get; set; }
    public decimal PreviousBalance { get; set; }
    public decimal NewBalance { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
