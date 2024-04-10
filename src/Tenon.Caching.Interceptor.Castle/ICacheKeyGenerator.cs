using System.Reflection;

namespace Tenon.Caching.Interceptor.Castle;

public interface ICacheKeyGenerator
{
    string GetCacheKey(MethodInfo methodInfo, object[] args, string prefix);

    string[] GetCacheKeys(MethodInfo methodInfo, object[] args, string prefix);

    string GetCacheKeyPrefix(MethodInfo methodInfo, string prefix);
}