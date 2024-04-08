using System.Collections;
using System.Reflection;

namespace Tenon.Caching.Interceptor.Castle;

internal sealed class ParameterCacheKeys
{
    public static string? GenerateCacheKey(object? parameter)
    {
        return parameter switch
        {
            null => string.Empty,
            string key => key,
            DateTime dateTime => dateTime.ToString("yyyyMMMMddHHmmss"),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("yyyyMMMMddHHmmss"),
            IEnumerable enumerable => GenerateCacheKey(enumerable.Cast<object>()),
            ParameterInfo parameterInfo => GenerateCacheKey(parameterInfo.Name),
            _ => parameter?.ToString()?.Replace(" ", "")
        };
    }

    private static string? GenerateCacheKey(IEnumerable<object>? parameter)
    {
        if (parameter == null) return string.Empty;
        return "[" + string.Join(",", parameter) + "]";
    }
}