using Domain.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Infrastructure.Locks
{
    public class RedisLockService : ILockService
    {
        private readonly ILogger<RedisLockService> _logger;
        private readonly IDatabase _db;
        private readonly string _instanceToken;

        public RedisLockService(ILogger<RedisLockService> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            _db = redis.GetDatabase();
            _instanceToken = Guid.NewGuid().ToString("N");
        }

        public async Task<bool> AcquireAsync(string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retryInterval, CancellationToken ct = default)
        {
            _logger.LogInformation(
                "Tentando adquirir o lock para recurso {Resource}",
                resource);

            var end = DateTime.UtcNow + wait;

            while (DateTime.UtcNow < end)
            {
                // SET resource value NX EX expiry
                var acquired = await _db.StringSetAsync(resource, _instanceToken, expiry, When.NotExists);
                if (acquired)
                {
                    _logger.LogInformation("Lock adquirido para {Resource}", resource);
                    return true;
                }

                _logger.LogWarning(
                "Lock em uso para {Resource}, tentando novamente em {RetryInterval} ms",
                resource,
                (int)wait.TotalMilliseconds);

                await Task.Delay(retryInterval, ct);
            }

            _logger.LogWarning(
                "Não foi possível adquirir o lock para {Resource} dentro de {Wait} ms",
                resource,
                (int)wait.TotalMilliseconds);

            return false;
        }

        public async Task ReleaseAsync(string resource)
        {
            var script = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";

            var result = (int)(long)await _db.ScriptEvaluateAsync(script, new RedisKey[] { resource }, new RedisValue[] { _instanceToken });

            if (result == 1)
                _logger.LogInformation("Lock liberado para {Resource}", resource);
            else
                _logger.LogWarning("Lock NÃO liberado para {Resource} (não era deste token)", resource);
        }
    }
}
