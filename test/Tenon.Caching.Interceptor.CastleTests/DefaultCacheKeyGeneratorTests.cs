using System.Reflection;
using Tenon.Caching.Interceptor.Castle;
using Tenon.Caching.Interceptor.Castle.Attributes;
using Xunit;

namespace Tenon.Caching.Interceptor.CastleTests;

public sealed class DefaultCacheKeyGeneratorTests
{
    private readonly DefaultCacheKeyGenerator _generator = new();

    [Fact]
    public void GetCacheKey_NoArgs_ReturnsPrefixWithTypeAndMethodAndZero()
    {
        var method = GetMethod(nameof(FakeTarget.GetNoArg));
        var key = _generator.GetCacheKey(method, Array.Empty<object>(), "");
        Assert.NotNull(key);
        Assert.Contains("FakeTarget", key);
        Assert.Contains("GetNoArg", key);
        Assert.EndsWith("0", key);
    }

    [Fact]
    public void GetCacheKey_WithPrefix_IncludesPrefix()
    {
        var method = GetMethod(nameof(FakeTarget.GetNoArg));
        var key = _generator.GetCacheKey(method, Array.Empty<object>(), "App");
        Assert.StartsWith("App:", key);
    }

    [Fact]
    public void GetCacheKey_WithArgs_SerializesAllParams()
    {
        var method = GetMethod(nameof(FakeTarget.GetWithArgs));
        var key = _generator.GetCacheKey(method, new object[] { "id1", 42 }, "");
        Assert.Contains("id1", key);
        Assert.Contains("42", key);
    }

    [Fact]
    public void GetCacheKey_WithCachingParameter_OnlyUsesMarkedParams()
    {
        var method = GetMethod(nameof(FakeTarget.GetWithCachingParam));
        var key = _generator.GetCacheKey(method, new object[] { "used", "ignored" }, "");
        Assert.Contains("used", key);
        Assert.DoesNotContain("ignored", key);
    }

    [Fact]
    public void GetCacheKeyPrefix_EmptyPrefix_ReturnsTypeAndMethodAndTrailingSeparator()
    {
        var method = GetMethod(nameof(FakeTarget.GetNoArg));
        var prefix = _generator.GetCacheKeyPrefix(method, "");
        Assert.Equal("FakeTarget:GetNoArg:", prefix);
    }

    [Fact]
    public void GetCacheKeyPrefix_WithPrefix_ReturnsPrefixTypeMethod()
    {
        var method = GetMethod(nameof(FakeTarget.GetNoArg));
        var prefix = _generator.GetCacheKeyPrefix(method, "P");
        Assert.Equal("P:FakeTarget:GetNoArg:", prefix);
    }

    [Fact]
    public void GetCacheKeys_WithArgs_ReturnsOneKeyPerArg()
    {
        var method = GetMethod(nameof(FakeTarget.GetWithArgs));
        var keys = _generator.GetCacheKeys(method, new object[] { "a", 1 }, "");
        Assert.Equal(2, keys.Length);
        Assert.Contains("a", keys[0]);
        Assert.Contains("1", keys[0]);
        Assert.Equal(keys[0], keys[1]);
    }

    private static MethodInfo GetMethod(string name)
    {
        return typeof(FakeTarget).GetMethod(name)!;
    }

    private static class FakeTarget
    {
        public static void GetNoArg() { }

        public static void GetWithArgs(string id, int num) { }

        public static void GetWithCachingParam([CachingParameter] string used, string ignored) { }
    }
}
