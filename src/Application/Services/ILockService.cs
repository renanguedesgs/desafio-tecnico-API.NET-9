namespace Application.Services
{
    public interface ILockService
    {
        Task<bool> AcquireAsync(
            string resource,
            TimeSpan expiry,
            TimeSpan wait,
            TimeSpan retryInterval,
            CancellationToken ct = default);

        Task ReleaseAsync(string resource);
    }
}