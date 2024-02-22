using System;
using System.Collections.Generic;

namespace Tenon.Caching.Configurations;

public class CachingOptions
{
    internal IList<ICachingOptionsExtension> Extensions { get; } = new List<ICachingOptionsExtension>();

    public void RegisterExtension(ICachingOptionsExtension extension)
    {
        if (extension == null)
            throw new ArgumentNullException(nameof(extension));

        Extensions.Add(extension);
    }
}