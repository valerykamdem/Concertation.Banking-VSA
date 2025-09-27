using Concertation.Banking.API.Shared.Interfaces;
using StackExchange.Redis;

namespace Concertation.Banking.API.Infrastructure.Redis;

public class RedisRefreshTokenService : IRefreshTokenService
{
    private readonly IDatabase _redis;

    public RedisRefreshTokenService(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task SaveTokenAsync(string userId, string refreshToken)
    {
        string key = $"refresh_token:{userId}:{refreshToken}";
        await _redis.StringSetAsync(key, "active", TimeSpan.FromDays(7));
    }

    public async Task<string?> GetTokenAsync(string userId, string refreshToken)
    {
        string key = $"refresh_token:{userId}:{refreshToken}";
        return await _redis.StringGetAsync(key);
    }

    public async Task DeleteTokenAsync(string userId, string refreshToken)
    {
        string key = $"refresh_token:{userId}:{refreshToken}";
        await _redis.KeyDeleteAsync(key);
    }
}
