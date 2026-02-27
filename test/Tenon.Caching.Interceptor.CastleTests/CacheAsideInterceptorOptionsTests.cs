using Tenon.Caching.Interceptor.Castle.Configurations;
using Xunit;

namespace Tenon.Caching.Interceptor.CastleTests;

public sealed class CacheAsideInterceptorOptionsTests
{
    [Fact]
    public void Default_DelayedDelete_IsOneSecond()
    {
        var options = new CacheAsideInterceptorOptions();
        Assert.Equal(TimeSpan.FromSeconds(1), options.DelayedDelete);
    }

    [Fact]
    public void DelayedDelete_CanBeSet()
    {
        var options = new CacheAsideInterceptorOptions
        {
            DelayedDelete = TimeSpan.FromMilliseconds(500)
        };
        Assert.Equal(TimeSpan.FromMilliseconds(500), options.DelayedDelete);
    }
}
