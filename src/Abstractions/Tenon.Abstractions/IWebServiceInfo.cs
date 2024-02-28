namespace Tenon.Abstractions;

public interface IWebServiceInfo : IServiceInfo
{
    public string CorsPolicy { get;  }

}