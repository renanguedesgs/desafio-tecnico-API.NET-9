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

    public async Task<bool> AcquireAsync(
        string resource,
        TimeSpan expiry,
        TimeSpan wait,
        TimeSpan retryInterval,
        CancellationToken ct = default)
    {
        var fullKey = Prefix + resource;
        var token = Guid.NewGuid().ToString("N");
        var end = DateTime.UtcNow + wait;

        while (DateTime.UtcNow < end)
        {
            var acquired = await _db.StringSetAsync(fullKey, token, expiry, When.NotExists);
            if (acquired)
                return true;

            await Task.Delay(retryInterval, ct);
        }

        return false;
    }

    public async Task ReleaseAsync(string resource)
    {
        var fullKey = Prefix + resource;
        await _db.KeyDeleteAsync(fullKey);
    }
}
