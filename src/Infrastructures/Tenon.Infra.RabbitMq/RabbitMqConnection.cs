using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tenon.Infra.RabbitMq.Configurations;

namespace Tenon.Infra.RabbitMq;

public sealed class RabbitMqConnection : IDisposable
{
    private static readonly object SyncRoot = new();
    private readonly Lazy<IConnectionFactory> _connectionFactory;
    private readonly ILogger<RabbitMqConnection> _logger;
    public readonly RabbitMqOptions Options;
    public IConnection? Connection { get; private set; }
    private bool _disposed;

    public RabbitMqConnection(RabbitMqOptions rabbitMqOptions, ILogger<RabbitMqConnection> logger)
    {
        Options = rabbitMqOptions ?? throw new ArgumentNullException(nameof(rabbitMqOptions));
        _connectionFactory = new Lazy<IConnectionFactory>(CreateConnectionFactory);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _disposed = false;
    }

    public RabbitMqConnection(IOptionsMonitor<RabbitMqOptions> rabbitMqOptions, ILogger<RabbitMqConnection> logger)
    {
        if (rabbitMqOptions == null)
            throw new ArgumentNullException(nameof(rabbitMqOptions));
        Options = rabbitMqOptions.CurrentValue;
        _connectionFactory = new Lazy<IConnectionFactory>(CreateConnectionFactory);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _disposed = false;
    }

    public bool IsConnected => (Connection?.IsOpen ?? false) && !_disposed;

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            if (Connection != null)
            {
                Connection.ConnectionShutdown -= OnConnectionShutdown;
                Connection.CallbackException -= OnCallbackException;
                Connection.ConnectionBlocked -= OnConnectionBlocked;
                Connection.Dispose();
                _disposed = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "RabbitMQ connections dispose failed");
        }
    }

    private IConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory
        {
            HostName = Options.HostName,
            Port = Options.Port,
            UserName = Options.UserName,
            Password = Options.Password,
            VirtualHost = Options.VirtualHost,
        };
    }

    public bool TryConnect()
    {
        if (!IsConnected)
            lock (SyncRoot)
            {
                if (!IsConnected)
                {
                    Connection = _connectionFactory.Value.CreateConnection();
                    if (IsConnected)
                    {
                        Connection.ConnectionShutdown += OnConnectionShutdown;
                        Connection.CallbackException += OnCallbackException;
                        Connection.ConnectionBlocked += OnConnectionBlocked;
                    }
                }
            }

        return IsConnected;
    }

    private void AutoReConnect()
    {
        if (!Options.AutoReConnect) return;
        if (_disposed)
        {
            _logger.LogWarning("RabbitMQ connection disposed,can not auto reconnect");
            return;
        }

        var reConnectResult = TryConnect();
        _logger.LogInformation($"RabbitMQ connection auto reconnect result:{reConnectResult}");
    }

    private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        _logger.LogWarning($"RabbitMQ connection is blocked,reason:{e.Reason}");
        AutoReConnect();
    }

    private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, $"RabbitMQ connection throw exception,detail:{e.Detail}");
        AutoReConnect();
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogError(e.Exception,
            $"RabbitMQ connection is blocked,replyCode:{e.ReplyCode},replyText:{e.ReplyText}");
        AutoReConnect();
    }
}