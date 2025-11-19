using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions;

public interface ILockService
{
    Task<bool> TryAcquireAsync(string key, TimeSpan ttl, CancellationToken ct = default);
    Task ReleaseAsync(string key);
}
