using Concertation.Banking.API.Infrastructure.Database;

namespace Concertation.Banking.API.Infrastructure.Logging;

public class AuditLogger
{
    private readonly AppDbContext _db;

    public AuditLogger(AppDbContext db) => _db = db;

    public async Task LogAsync(string userId, string action, string description)
    {
        await _db.AuditLogs.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = action,
            Description = description
        });

        await _db.SaveChangesAsync();
    }
}
