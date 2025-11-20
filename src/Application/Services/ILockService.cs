namespace Application.Services
{
    public interface ILockService
    {
        Task<LockResult> AcquireAsync(
            string resourceName,
            TimeSpan duration,
            CancellationToken cancellationToken = default);

        Task ReleaseAsync(string resourceName, string lockToken);
    }

    public record LockResult(
        bool IsAcquired,
        string LockToken,
        DateTime ExpirationTime);
}
