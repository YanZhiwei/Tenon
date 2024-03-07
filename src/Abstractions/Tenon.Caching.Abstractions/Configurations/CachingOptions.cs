#nullable enable
using System;
using System.Collections.Generic;

namespace Tenon.Caching.Abstractions.Configurations;

public class CachingOptions
{
    public CachingOptions()
    {
        MaxRandomSecond = 5;
    }
    internal IList<ICachingOptionsExtension> Extensions { get; } = new List<ICachingOptionsExtension>();

    public string? KeyedServiceKey { get; set; }
    public int MaxRandomSecond { get; set; }
    public string Prefix { get; set; }

    public void RegisterExtension(ICachingOptionsExtension extension)
    {
        if (extension == null)
            throw new ArgumentNullException(nameof(extension));

        Extensions.Add(extension);
    }
}