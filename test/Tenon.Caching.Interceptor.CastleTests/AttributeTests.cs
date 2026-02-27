using Tenon.Caching.Interceptor.Castle.Attributes;
using Xunit;

namespace Tenon.Caching.Interceptor.CastleTests;

public sealed class AttributeTests
{
    [Fact]
    public void CachingAblAttribute_DefaultExpirationInSec_Is30()
    {
        var attr = new CachingAblAttribute();
        Assert.Equal(30, attr.ExpirationInSec);
    }

    [Fact]
    public void CachingAblAttribute_ExpirationInSec_CanBeSet()
    {
        var attr = new CachingAblAttribute { ExpirationInSec = 3600 };
        Assert.Equal(3600, attr.ExpirationInSec);
    }

    [Fact]
    public void CachingEvictAttribute_DefaultCacheKeys_IsEmpty()
    {
        var attr = new CachingEvictAttribute();
        Assert.NotNull(attr.CacheKeys);
        Assert.Empty(attr.CacheKeys);
    }

    [Fact]
    public void CachingEvictAttribute_CacheKeys_CanBeSet()
    {
        var attr = new CachingEvictAttribute { CacheKeys = new[] { "k1", "k2" } };
        Assert.Equal(2, attr.CacheKeys.Length);
        Assert.Contains("k1", attr.CacheKeys);
        Assert.Contains("k2", attr.CacheKeys);
    }

    [Fact]
    public void CachingInterceptorAttribute_DefaultIsHighAvailability_IsTrue()
    {
        var attr = new CachingAblAttribute();
        Assert.True(attr.IsHighAvailability);
    }
}
