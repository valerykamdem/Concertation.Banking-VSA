using Concertation.Banking.API.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Concertation.Banking.API.Features.Accounts.Handlers;

public class CheckUserRoleHandler
{
    private readonly AppDbContext _db;

    public CheckUserRoleHandler(AppDbContext db) => _db = db;

    public async Task<bool> OwnsAccountAsync(string userId, Guid accountId)
    {
        return await _db.Accounts
            .AnyAsync(a => a.Id == accountId && a.UserId == Guid.Parse(userId));
    }

    public async Task<bool> IsAdminAsync(string userId)
    {
        return await _db.UserRoles
            .AnyAsync(ur => ur.UserId == Guid.Parse(userId) && ur.Role.Name == "Admin");
    }
}
