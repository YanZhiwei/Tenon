using System.Reflection;
using System.Text;
using Tenon.Caching.Interceptor.Castle.Attributes;

namespace Tenon.Caching.Interceptor.Castle;

public sealed class DefaultCacheKeyBuilder : ICacheKeyBuilder
{
    private const char SeparatorChar = ':';

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

    public string[] GetCacheKeys(MethodInfo methodInfo, object[] args, string prefix)
    {
        var cacheKeys = new List<string>();
        foreach (var arg in args)
            cacheKeys.Add(arg.GetType().IsArray
                ? GetCacheKey(methodInfo, (object[])arg, prefix)
                : GetCacheKey(methodInfo, args, prefix));
        return cacheKeys.ToArray();
    }

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