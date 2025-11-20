using Domain.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

public class RedisLockService : ILockService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly string _namespacePrefix;
    private readonly ILogger<RedisLockService> _logger;

    public RedisLockService(
        IConnectionMultiplexer redis,
        ILogger<RedisLockService> logger,
        IConfiguration config)
    {
        _redis = redis;
        _logger = logger;
        _namespacePrefix = config.GetValue<string>("LockNamespace") ?? "locks:";
    }

    private string Namespaced(string key) => $"{_namespacePrefix}{key}";

    public async Task<bool> TryAcquireAsync(string key, TimeSpan ttl, CancellationToken ct = default)
    {
        var db = _redis.GetDatabase();
        var namespacedKey = Namespaced(key);

        var acquired = await db.StringSetAsync(namespacedKey, "locked", ttl, When.NotExists);
        _logger.LogInformation("Tentando adquirir o lock para {Key}: {Result}", namespacedKey, acquired ? "SUCESSO" : "OCUPADO");

        return acquired;
    }

    public async Task ReleaseAsync(string key, CancellationToken ct = default)
    {
        var db = _redis.GetDatabase();
        var namespacedKey = Namespaced(key);
        await db.KeyDeleteAsync(namespacedKey);
        _logger.LogInformation("Lock liberado para {Key}", namespacedKey);
    }
}
