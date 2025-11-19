namespace Application.Services
{
    public interface ILockService
    {
        Task<LockResult> AcquireAsync(
            string resource,
            TimeSpan expiry,
            CancellationToken ct = default);

        Task ReleaseAsync(string resource, string lockToken);
    }

    public record LockResult(bool Acquired, string LockToken, DateTime ExpiresAt);
}
