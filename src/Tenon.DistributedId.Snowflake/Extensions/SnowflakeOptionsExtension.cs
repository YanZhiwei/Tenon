using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenon.DistributedId.Abstractions;
using Tenon.DistributedId.Snowflake.Configurations;

namespace Tenon.DistributedId.Snowflake.Extensions;

public sealed class SnowflakeOptionsExtension : IDistributedIdOptionsExtension
{
    private readonly IConfigurationSection _snowflakeIdOptionsSection;

    public SnowflakeOptionsExtension(IConfigurationSection snowflakeIdOptionsSection)
    {
        _snowflakeIdOptionsSection = snowflakeIdOptionsSection ??
                                     throw new ArgumentNullException(nameof(snowflakeIdOptionsSection));
    }

    public void AddServices(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        var snowflakeIdOptions = _snowflakeIdOptionsSection.Get<SnowflakeIdOptions>();
        if (snowflakeIdOptions == null)
            throw new ArgumentNullException(nameof(SnowflakeIdOptions));
        if (string.IsNullOrWhiteSpace(snowflakeIdOptions.ServiceName))
            throw new ArgumentNullException(nameof(snowflakeIdOptions.ServiceName));
        services.Configure<SnowflakeIdOptions>(_snowflakeIdOptionsSection);
        services.AddSingleton<IDGenerator, SnowflakeIdGenerator>();
    }
}