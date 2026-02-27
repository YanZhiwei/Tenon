namespace Tenon.Caching.Interceptor.Castle.Exceptions;

/// <summary>
/// 缓存失效（删除）失败时抛出，携带待删除的键。
/// </summary>
internal sealed class CachingEvictException(string message, string[] needRemovedKeys, Exception innerException)
    : Exception(message, innerException)
{
    /// <summary>
    /// 本次待删除的缓存键。
    /// </summary>
    public string[] RemovedKeys { get; } = needRemovedKeys ?? throw new ArgumentNullException(nameof(needRemovedKeys));
}