using Domain.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

public class RedisLockService : ILockService
{
    private readonly IConnectionMultiplexer connection;
    private readonly ILogger<RedisLockService> logger;
    private readonly string lockNamespace;

    public RedisLockService(
        IConnectionMultiplexer connection,
        ILogger<RedisLockService> logger,
        IConfiguration configuration)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        lockNamespace = configuration.GetValue<string>("LockNamespace") ?? "locks:";
    }

    private string BuildKey(string resourceName) => $"{lockNamespace}{resourceName}";

    public async Task<bool> TryAcquireAsync(
        string resourceName,
        TimeSpan lockDuration,
        CancellationToken cancellationToken = default)
    {
        var db = connection.GetDatabase();
        var key = BuildKey(resourceName);
        var token = Guid.NewGuid().ToString("N");

        var acquired = await db.StringSetAsync(
            key,
            token,
            lockDuration,
            When.NotExists);

        logger.LogInformation(
            "Lock attempt for {Key}: {Result}",
            key,
            acquired ? "ACQUIRED" : "BUSY");

        return acquired;
    }

    public async Task ReleaseAsync(
        string resourceName,
        CancellationToken cancellationToken = default)
    {
        var db = connection.GetDatabase();
        var key = BuildKey(resourceName);

        await db.KeyDeleteAsync(key);

        logger.LogInformation("Lock released for {Key}", key);
    }
}
