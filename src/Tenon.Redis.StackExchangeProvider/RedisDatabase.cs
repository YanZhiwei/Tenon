using System.Net;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tenon.Redis.StackExchangeProvider.Configurations;

namespace Tenon.Redis.StackExchangeProvider;

public class RedisDatabase
{
    private readonly RedisOptions _redisOptions;
    protected readonly Lazy<ConnectionMultiplexer> ConnectionMultiplexer;

    public RedisDatabase(IOptionsMonitor<RedisOptions> options)
    {
        _redisOptions = options.CurrentValue;
        ConnectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
    }

    private ConnectionMultiplexer CreateConnectionMultiplexer()
    {
        _ = ConfigurationOptions.Parse(_redisOptions.ConnectionString);
        return StackExchange.Redis.ConnectionMultiplexer.Connect(_redisOptions.ConnectionString);
    }

    public IDatabase GetDatabase()
    {
        return ConnectionMultiplexer.Value.GetDatabase();
    }

    public virtual IEnumerable<IServer> GetServers()
    {
        var endpoints = GetMastersServers();

        foreach (var endpoint in endpoints)
            yield return ConnectionMultiplexer.Value.GetServer(endpoint);
    }

    protected virtual IEnumerable<EndPoint> GetMastersServers()
    {
        var masters = new List<EndPoint>();
        foreach (var ep in ConnectionMultiplexer.Value.GetEndPoints())
        {
            var server = ConnectionMultiplexer.Value.GetServer(ep);
            if (!server.IsConnected) continue;
            if (server.ServerType == ServerType.Cluster)
            {
                var clusterConfiguration = server.ClusterConfiguration;
                if (clusterConfiguration is null)
                    throw new NullReferenceException(nameof(server.ClusterConfiguration));

                var nodes = clusterConfiguration.Nodes.Where(n => !n.IsReplica);
                if (nodes is null)
                    throw new NullReferenceException(nameof(server.ClusterConfiguration.Nodes));

                var endpoints = nodes.Select(n => n.EndPoint);
                if (endpoints is null)
                    throw new NullReferenceException(nameof(endpoints));

                masters.AddRange((IEnumerable<EndPoint>)endpoints);

                break;
            }

            if (server is { ServerType: ServerType.Standalone, IsReplica: false })
            {
                masters.Add(ep);
                break;
            }
        }

        return masters;
    }
}