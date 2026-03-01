using System.Reflection;
using System.Text;
using Tenon.Caching.Interceptor.Castle.Attributes;

namespace Tenon.Caching.Interceptor.Castle.KeyGenerators;

/// <summary>
/// 默认缓存键生成器：前缀 + 类型名 + 方法名 + 参数片段，用 : 连接。
/// </summary>
public sealed class DefaultCacheKeyGenerator : ICacheKeyGenerator
{
    private const char SeparatorChar = ':';

    /// <summary>
    /// 生成单条缓存键。若有 <see cref="CachingParameterAttribute" /> 则仅用标记参数，否则用全部参数。
    /// </summary>
    /// <param name="methodInfo">目标方法。</param>
    /// <param name="args">方法参数。</param>
    /// <param name="prefix">键前缀（可选）。</param>
    /// <returns>完整缓存键。</returns>
    public string GetCacheKey(MethodInfo methodInfo, object[] args, string prefix)
    {
        IEnumerable<string?> methodArguments = new[] { "0" };
        if (!(args?.Any() ?? false)) return CreateCacheKey(methodInfo, prefix, methodArguments);
        var cacheParams = methodInfo.GetParameters()
            .Where(x => x.GetCustomAttribute<CachingParameterAttribute>() != null)
            .Select(x => x.Position)?.ToArray();
        if (cacheParams?.Any() ?? false)
            methodArguments = args.Where(x => cacheParams.Contains(Array.IndexOf(args, x)))
                .Select(ParameterCacheKeys.GenerateCacheKey);
        else
            methodArguments = args.Select(ParameterCacheKeys.GenerateCacheKey);

        return CreateCacheKey(methodInfo, prefix, methodArguments);
    }

    /// <summary>
    /// 生成多条缓存键（按参数逐条或按数组元素展开），用于批量失效等。
    /// </summary>
    /// <param name="methodInfo">目标方法。</param>
    /// <param name="args">方法参数。</param>
    /// <param name="prefix">键前缀。</param>
    /// <returns>缓存键数组。</returns>
    public string[] GetCacheKeys(MethodInfo methodInfo, object[] args, string prefix)
    {
        var cacheKeys = new List<string>();
        foreach (var arg in args)
            cacheKeys.Add(arg.GetType().IsArray
                ? GetCacheKey(methodInfo, (object[])arg, prefix)
                : GetCacheKey(methodInfo, args, prefix));
        return cacheKeys.ToArray();
    }

    /// <summary>
    /// 生成键前缀：prefix + 类型名 + 方法名 + 尾部分隔符。
    /// </summary>
    /// <param name="methodInfo">目标方法。</param>
    /// <param name="prefix">自定义前缀，可为空。</param>
    /// <returns>前缀字符串。</returns>
    public string GetCacheKeyPrefix(MethodInfo methodInfo, string prefix)
    {
        var builder = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(prefix))
            builder.Append($"{prefix}{SeparatorChar}");
        var typeName = methodInfo.DeclaringType?.Name;
        var methodName = methodInfo.Name;
        builder.Append($"{typeName}{SeparatorChar}{methodName}{SeparatorChar}");
        return builder.ToString();
    }

    /// <summary>
    /// 将前缀与参数片段用分隔符拼接成完整缓存键。
    /// </summary>
    private string CreateCacheKey(MethodInfo methodInfo, string prefix, IEnumerable<string> parameters)
    {
        var methodName = methodInfo.Name;
        var cacheKeyPrefix = GetCacheKeyPrefix(methodInfo, prefix);
        var builder = new StringBuilder();
        builder.Append(cacheKeyPrefix);
        builder.Append(string.Join(SeparatorChar.ToString(), parameters));
        return builder.ToString();
    }
}
