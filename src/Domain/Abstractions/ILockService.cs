using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions;

public interface ILockService
{
    Task<bool> AcquireAsync(string resource, TimeSpan expiry, TimeSpan wait, TimeSpan retryInterval, CancellationToken ct = default);
    Task ReleaseAsync(string resource);
}
