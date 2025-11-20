using Domain.Abstractions;
using StackExchange.Redis;

namespace Infrastructure.Concurrency;

public class RedisLockService : ILockService
{
    private readonly IDatabase _db;
    private const string Prefix = "locks:";

    public RedisLockService(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    public async Task<bool> TryAcquireAsync(string key, TimeSpan ttl, CancellationToken ct = default)
    {
        var fullKey = Prefix + key;
        var token = Guid.NewGuid().ToString("N");

        var acquired = await _db.StringSetAsync(fullKey, token, ttl, When.NotExists);
        return acquired;
    }

    public async Task ReleaseAsync(string key, CancellationToken ct = default)
    {
        var fullKey = Prefix + key;
        await _db.KeyDeleteAsync(fullKey);
    }

    public async Task<bool> AcquireWithRetryAsync(
        string key,
        TimeSpan ttl,
        TimeSpan wait,
        TimeSpan retryInterval,
        CancellationToken ct = default)
    {
        var end = DateTime.UtcNow + wait;
        while (DateTime.UtcNow < end)
        {
            if (await TryAcquireAsync(key, ttl, ct))
                return true;

            await Task.Delay(retryInterval, ct);
        }
        return false;
    }
}
