namespace Tenon.Abstractions;

public interface IServiceDescriptor
{
    public string Id { get; }

    public string ServiceName { get; }

    public string Version { get; }

    public string Description { get; }
}