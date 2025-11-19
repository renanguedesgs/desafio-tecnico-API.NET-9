using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        return await _db.StringSetAsync(fullKey, token, ttl, When.NotExists);
    }

    public async Task ReleaseAsync(string key)
    {
        var fullKey = Prefix + key;
        await _db.KeyDeleteAsync(fullKey);
    }
}
