#nullable enable
using System;
using System.Collections.Generic;

namespace Tenon.BloomFilter.Abstractions.Configurations;

public sealed class BloomFilterOptions
{
    /// <summary>
    ///     过滤器名字
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     容错率
    /// </summary>
    public double ErrorRate { get; set; }

    /// <summary>
    ///     容积
    /// </summary>
    public int Capacity { get; set; }

    internal IList<IBloomFilterOptionsExtension> Extensions { get; } = new List<IBloomFilterOptionsExtension>();

    public bool KeyedServices { get; set; } = false;

    public string? KeyedServiceKey { get; set; }

    public void RegisterExtension(IBloomFilterOptionsExtension extension)
    {
        if (extension == null)
            throw new ArgumentNullException(nameof(extension));

        Extensions.Add(extension);
    }
}