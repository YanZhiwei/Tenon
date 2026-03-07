using Microsoft.Extensions.Hosting;
using Tenon.Infra.Consul.Extensions;
using Xunit;

namespace Tenon.Infra.ConsulTests;

/// <summary>
/// 验证 UseConsulRegistrationCenter 的 null 校验。
/// </summary>
public sealed class HostExtensionTests
{
    [Fact]
    public void UseConsulRegistrationCenter_ThrowsArgumentNullException_WhenHostIsNull()
    {
        static Uri GetAddress() => new("http://localhost:5000");

        var ex = Assert.Throws<ArgumentNullException>(() =>
            HostExtension.UseConsulRegistrationCenter(null!, GetAddress));

        Assert.Equal("host", ex.ParamName);
    }

    [Fact]
    public void UseConsulRegistrationCenter_ThrowsArgumentNullException_WhenGetServiceAddressHandleIsNull()
    {
        using var host = Host.CreateDefaultBuilder().Build();

        var ex = Assert.Throws<ArgumentNullException>(() =>
            HostExtension.UseConsulRegistrationCenter(host, null!));

        Assert.Equal("getServiceAddressHandle", ex.ParamName);
    }
}
