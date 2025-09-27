
using System;
using Concertation.Banking.API.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Concertation.Banking.API.Infrastructure.Services;

public class AccountNumberGenerator : IAccountNumberGenerator
{

    private readonly AppDbContext _db;
    private readonly Random _random = new();

    public AccountNumberGenerator(AppDbContext db) => _db = db;

    public async Task<string> GenerateUniqueAccountNumber(string prefix)
    {
        string number;
        do
        {
            string timestamp = DateTime.UtcNow.ToString("yyMMddHHmm");
            int random = _random.Next(100000, 999999);
            number = $"{prefix}-{timestamp}-{random}";
        } while (await _db.Accounts.AnyAsync(a => a.AccountNumber == number));

        return number;
    }
}
