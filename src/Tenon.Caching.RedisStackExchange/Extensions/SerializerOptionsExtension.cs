﻿using Microsoft.Extensions.DependencyInjection;
using Tenon.Caching.Abstractions;
using Tenon.Caching.Abstractions.Configurations;
using Tenon.Serialization.Json.Extensions;

namespace Tenon.Caching.RedisStackExchange.Extensions;

internal class SerializerOptionsExtension(bool useSystemTextJson, CachingOptions options)
    : ICachingOptionsExtension
{
    public void AddServices(IServiceCollection services)
    {
        if (!string.IsNullOrWhiteSpace(options.KeyedServiceKey))
        {
            if (useSystemTextJson)
                services.AddKeyedSystemTextJsonSerializer(options.KeyedServiceKey);
        }
        else
        {
            if (useSystemTextJson)
                services.AddSystemTextJsonSerializer();
        }
    }
}