using System.Text.Json;
using Concertation.Banking.API.Features.Transactions.Models;
using StackExchange.Redis;

namespace Concertation.Banking.API.Shared.Interfaces;

public interface IPendingTransactionRedisStore
{
    Task SaveAsync(PendingTransaction pending, TimeSpan? expiry = null);
    Task<PendingTransaction?> GetAsync(string transferId);
    Task RemoveAsync(string transferId);
}

public class PendingTransactionRedisStore : IPendingTransactionRedisStore
{
    private readonly IDatabase _redisDb;
    private const string Prefix = "transfer:preview:";

    public PendingTransactionRedisStore(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task SaveAsync(PendingTransaction pending, TimeSpan? expiry = null)
    {
        string key = Prefix + pending.TransferId;
        string json = JsonSerializer.Serialize(pending);
        await _redisDb.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(10));
    }

    public async Task<PendingTransaction?> GetAsync(string transferId)
    {
        string key = Prefix + transferId;
        RedisValue json = await _redisDb.StringGetAsync(key);
        if (json.IsNullOrEmpty)
            return null;
        return JsonSerializer.Deserialize<PendingTransaction>(json!);
    }

    public async Task RemoveAsync(string transferId)
    {
        string key = Prefix + transferId;
        await _redisDb.KeyDeleteAsync(key);
    }
}

