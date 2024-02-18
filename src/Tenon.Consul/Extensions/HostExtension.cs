using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tenon.Abstractions;

namespace Tenon.Consul.Extensions;

public static class HostExtension
{
    public static IHost UseConsulRegistrationCenter(this IHost host)
    {
        if (host == null)
            throw new ArgumentNullException(nameof(host));

        var serviceInfo = host.Services.GetRequiredService<IWebServiceInfo>();
        var registration = ActivatorUtilities.CreateInstance<RegistrationProvider>(host.Services);
        registration.Register(serviceInfo.Id);
        return host;
    }
}