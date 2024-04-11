using System.Collections.Concurrent;

namespace Tenon.Caching.Interceptor.Castle;

public sealed class CachingEvictFailedQueue
{
    private static readonly Lazy<CachingEvictFailedQueue> Lazy = new(() => new CachingEvictFailedQueue());
    private static readonly ConcurrentQueue<string[]> FailedQueue;

    static CachingEvictFailedQueue()
    {
        FailedQueue = new ConcurrentQueue<string[]>();
    }

    private CachingEvictFailedQueue()
    {
    }

    public static CachingEvictFailedQueue Instance => Lazy.Value;

    public void Enqueue(string[] keys)
    {
        if (keys?.Any() ?? false)
            FailedQueue.Enqueue(keys);


    }

    public bool TryDequeue(out string[]? keys)
    {
        return FailedQueue.TryDequeue(out keys);
    }
}