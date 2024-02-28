#nullable enable
using System;
using System.Collections.Generic;

namespace Tenon.BloomFilter.Abstractions.Configurations
{
    public sealed class BloomFilterOptions
    {
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
}