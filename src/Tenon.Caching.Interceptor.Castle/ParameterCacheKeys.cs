using System.Collections;
using System.Reflection;

namespace Tenon.Caching.Interceptor.Castle;

/// <summary>
/// 根据方法参数生成缓存键片段（用于拼接到完整 key）。
/// </summary>
internal sealed class ParameterCacheKeys
{
    /// <summary>
    /// 根据单个参数值生成键片段；集合会格式化为 [a,b,c]。
    /// </summary>
    /// <param name="parameter">参数值，可为 null。</param>
    /// <returns>键片段，null 或空集合返回空字符串。</returns>
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

    /// <summary>
    /// 将参数集合格式化为 [item1,item2,...] 形式的键片段。
    /// </summary>
    private static string? GenerateCacheKey(IEnumerable<object>? parameter)
    {
        if (parameter == null) return string.Empty;
        return "[" + string.Join(",", parameter) + "]";
    }
}