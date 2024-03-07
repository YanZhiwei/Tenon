using System;
using System.Collections.Generic;

namespace Tenon.DistributedLocker.Abstractions.Configurations;

public class DistributedLockerOptions
{
    public string KeyPrefix { get; set; } = string.Empty;
    public string KeyedServiceKey { get; set; }

    internal IList<IDistributedLockerOptionsExtension> Extensions { get; } =
        new List<IDistributedLockerOptionsExtension>();

    public void RegisterExtension(IDistributedLockerOptionsExtension extension)
    {
        if (extension == null)
            throw new ArgumentNullException(nameof(extension));

        Extensions.Add(extension);
    }
}