using System.Reflection;

namespace Tenon.Abstractions;

public interface IServiceInfo
{
    public string Id { get; }

    public string ServiceName { get; }

    public string Version { get; }

    public string Description { get; }
}