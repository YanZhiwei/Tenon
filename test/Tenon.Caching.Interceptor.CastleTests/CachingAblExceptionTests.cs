using Tenon.Caching.Interceptor.Castle.Exceptions;
using Xunit;

namespace Tenon.Caching.Interceptor.CastleTests;

public sealed class CachingAblExceptionTests
{
    [Fact]
    public void Constructor_SetsMessageAndCacheKeyAndInnerException()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new CachingAblException("msg", "cacheKey1", inner);
        Assert.Equal("msg", ex.Message);
        Assert.Equal("cacheKey1", ex.CacheKey);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void Constructor_ThrowsWhenCacheKeyNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CachingAblException("m", null!, new Exception()));
    }
}
