using System.Reflection;

namespace Tenon.Caching.Interceptor.Castle.KeyGenerators;

/// <summary>
/// 根据方法与参数生成缓存键。
/// </summary>
public interface ICacheKeyGenerator
{
    /// <summary>
    /// 生成单条缓存键。
    /// </summary>
    /// <param name="methodInfo">目标方法。</param>
    /// <param name="args">方法参数。</param>
    /// <param name="prefix">键前缀（可选）。</param>
    /// <returns>完整缓存键。</returns>
    string GetCacheKey(MethodInfo methodInfo, object[] args, string prefix);

    /// <summary>
    /// 生成多条缓存键（用于批量失效等）。
    /// </summary>
    /// <param name="methodInfo">目标方法。</param>
    /// <param name="args">方法参数。</param>
    /// <param name="prefix">键前缀。</param>
    /// <returns>缓存键数组。</returns>
    string[] GetCacheKeys(MethodInfo methodInfo, object[] args, string prefix);

    /// <summary>
    /// 生成键前缀（类型名:方法名）。
    /// </summary>
    /// <param name="methodInfo">目标方法。</param>
    /// <param name="prefix">自定义前缀。</param>
    /// <returns>前缀字符串。</returns>
    string GetCacheKeyPrefix(MethodInfo methodInfo, string prefix);
}
