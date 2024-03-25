using System.Net;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tenon.Infra.Redis.Configurations;

namespace Tenon.Infra.Redis.StackExchangeProvider;

public sealed class RedisConnection
{
    private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;
    private readonly RedisOptions _redisOptions;

    public RedisConnection(RedisOptions redisOptions)
    {
        _redisOptions = redisOptions ?? throw new ArgumentNullException(nameof(redisOptions));
        _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
    }

    public RedisConnection(IOptionsMonitor<RedisOptions> redisOptions)
    {
        if (redisOptions == null)
            throw new ArgumentNullException(nameof(redisOptions));
        _redisOptions = redisOptions.CurrentValue;
        _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
    }

    private ConnectionMultiplexer CreateConnectionMultiplexer()
    {
        var config = ConfigurationOptions.Parse(_redisOptions.ConnectionString);
        //当设置了 AbortOnConnectFail 为 true 时，如果在尝试连接服务时失败了，客户端会立即抛出异常并中断后续的操作。如果设置为 false，则即使连接失败，客户端也会尝试一些重连策略。
        //config.AbortOnConnectFail = false;
        return ConnectionMultiplexer.Connect(config);
    }

    public IDatabase GetDatabase()
    {
        return _connectionMultiplexer.Value.GetDatabase();
    }

    public IEnumerable<IServer> GetServers()
    {
        var endpoints = GetMastersServers();

        foreach (var endpoint in endpoints)
            yield return _connectionMultiplexer.Value.GetServer(endpoint);
    }

    private IEnumerable<EndPoint> GetMastersServers()
    {
        var masters = new List<EndPoint>();
        foreach (var ep in _connectionMultiplexer.Value.GetEndPoints())
        {
            var server = _connectionMultiplexer.Value.GetServer(ep);
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