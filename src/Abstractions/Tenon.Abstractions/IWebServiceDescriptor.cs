namespace Tenon.Abstractions;

public interface IWebServiceDescriptor : IServiceDescriptor
{
    public string CorsPolicy { get; }
}