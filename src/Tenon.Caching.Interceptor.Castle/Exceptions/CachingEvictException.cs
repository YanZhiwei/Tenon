namespace Tenon.Caching.Interceptor.Castle.Exceptions;

internal sealed class CachingEvictException(string message, string[] needRemovedKeys, Exception innerException)
    : Exception(message, innerException)
{
    public string[] RemovedKeys= needRemovedKeys??throw new ArgumentNullException(nameof(needRemovedKeys));
}