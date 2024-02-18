﻿using Tenon.Helper.Internal;

namespace Tenon.Consul.LoadBalancer;

internal sealed class RandomLoadBalancer : ILoadBalancer
{
    public string Resolve(IReadOnlyCollection<string> services)
    {
        var index = RandomHelper.NextNumber(0, services.Count);
       return services.ElementAt(index);
       
    }
}

