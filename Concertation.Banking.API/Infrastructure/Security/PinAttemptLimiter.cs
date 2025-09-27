using StackExchange.Redis;

namespace Concertation.Banking.API.Infrastructure.Security;

public class PinAttemptLimiter
{
    private readonly IDatabase _redis;

    public PinAttemptLimiter(IConnectionMultiplexer redis) => _redis = redis.GetDatabase();

    public async Task<bool> IsBlockedAsync(string userId)
    {
        string key = $"pin_attempts:{userId}";
        return await _redis.KeyExistsAsync(key);
    }

    public async Task RecordFailedAttemptAsync(string userId)
    {
        string key = $"pin_attempts:{userId}";
        await _redis.StringSetAsync(key, "blocked", TimeSpan.FromMinutes(15));
    }

    public async Task ResetAttemptsAsync(string userId)
    {
        string key = $"pin_attempts:{userId}";
        await _redis.KeyDeleteAsync(key);
    }
}
