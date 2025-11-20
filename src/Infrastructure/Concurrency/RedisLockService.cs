using Domain.Abstractions;
using StackExchange.Redis;

namespace Infrastructure.Concurrency;

public class RedisLockService : ILockService
{
    private readonly IDatabase database;
    private const string LockPrefix = "locks:";

    public RedisLockService(IConnectionMultiplexer connection)
    {
        database = connection.GetDatabase();
    }

    public async Task<bool> TryAcquireAsync(
        string resourceName,
        TimeSpan lockDuration,
        CancellationToken cancellationToken = default)
    {
        var key = BuildKey(resourceName);
        var token = Guid.NewGuid().ToString("N");

        return await database.StringSetAsync(
            key,
            token,
            lockDuration,
            When.NotExists);
    }

    public async Task ReleaseAsync(
        string resourceName,
        CancellationToken cancellationToken = default)
    {
        var key = BuildKey(resourceName);
        await database.KeyDeleteAsync(key);
    }

    public async Task<bool> AcquireWithRetryAsync(
        string resourceName,
        TimeSpan lockDuration,
        TimeSpan maxWaitTime,
        TimeSpan retryInterval,
        CancellationToken cancellationToken = default)
    {
        var deadline = DateTime.UtcNow + maxWaitTime;

        while (DateTime.UtcNow < deadline)
        {
            if (await TryAcquireAsync(resourceName, lockDuration, cancellationToken))
                return true;

            await Task.Delay(retryInterval, cancellationToken);
        }

        return false;
    }

    private static string BuildKey(string resourceName) => $"{LockPrefix}{resourceName}";
}
