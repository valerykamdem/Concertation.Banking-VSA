using System.Text.Json;
using Concertation.Banking.API.Features.Transactions.Models;
using Concertation.Banking.API.Features.Users.models;
using StackExchange.Redis;

namespace Concertation.Banking.API.Shared.Interfaces;

public interface IPendingUpdateUserRedisStore
{
    Task SaveAsync(PendingUpdateUser pending, TimeSpan? expiry = null);
    Task<PendingUpdateUser?> GetAsync(string transferId);
    Task RemoveAsync(string transferId);
}

public class PendingUpdateUserRedisStore : IPendingUpdateUserRedisStore
{
    private readonly IDatabase _redisDb;
    private const string Prefix = "updateUser:preview:";

    public PendingUpdateUserRedisStore(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task SaveAsync(PendingUpdateUser pending, TimeSpan? expiry = null)
    {
        string key = Prefix + pending.UserId;
        string json = JsonSerializer.Serialize(pending);
        await _redisDb.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(10));
    }

    public async Task<PendingUpdateUser?> GetAsync(string userId)
    {
        string key = Prefix + userId;
        RedisValue json = await _redisDb.StringGetAsync(key);
        if (json.IsNullOrEmpty)
            return null;
        return JsonSerializer.Deserialize<PendingUpdateUser>(json!);
    }

    public async Task RemoveAsync(string userId)
    {
        string key = Prefix + userId;
        await _redisDb.KeyDeleteAsync(key);
    }
}


