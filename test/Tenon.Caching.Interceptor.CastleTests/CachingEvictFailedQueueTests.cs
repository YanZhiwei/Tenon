using Tenon.Caching.Interceptor.Castle;
using Xunit;

namespace Tenon.Caching.Interceptor.CastleTests;

public sealed class CachingEvictFailedQueueTests
{
    [Fact]
    public void Instance_IsNotNull()
    {
        Assert.NotNull(CachingEvictFailedQueue.Instance);
    }

    [Fact]
    public void Instance_IsSingleton()
    {
        Assert.Same(CachingEvictFailedQueue.Instance, CachingEvictFailedQueue.Instance);
    }

    [Fact]
    public void Enqueue_ThenTryDequeue_ReturnsTrueAndKeys()
    {
        var keys = new[] { "key1", "key2" };
        CachingEvictFailedQueue.Instance.Enqueue(keys);
        var ok = CachingEvictFailedQueue.Instance.TryDequeue(out var outKeys);
        Assert.True(ok);
        Assert.NotNull(outKeys);
        Assert.Equal(keys, outKeys);
    }

    [Fact]
    public void TryDequeue_WhenEmpty_ReturnsFalse()
    {
        while (CachingEvictFailedQueue.Instance.TryDequeue(out _)) { }
        var ok = CachingEvictFailedQueue.Instance.TryDequeue(out var keys);
        Assert.False(ok);
        Assert.Null(keys);
    }

    [Fact]
    public void Enqueue_NullOrEmpty_DoesNotThrow()
    {
        CachingEvictFailedQueue.Instance.Enqueue(null!);
        CachingEvictFailedQueue.Instance.Enqueue(Array.Empty<string>());
    }
}
