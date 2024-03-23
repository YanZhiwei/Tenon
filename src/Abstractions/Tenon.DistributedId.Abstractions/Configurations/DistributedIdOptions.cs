using System;
using System.Collections.Generic;

namespace Tenon.DistributedId.Abstractions.Configurations;

public sealed class DistributedIdOptions
{
    internal IList<IDistributedIdOptionsExtension> Extensions { get; } = new List<IDistributedIdOptionsExtension>();

    public void RegisterExtension(IDistributedIdOptionsExtension extension)
    {
        if (extension == null)
            throw new ArgumentNullException(nameof(extension));

        Extensions.Add(extension);
    }
}