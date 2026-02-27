using System.Collections.Concurrent;

namespace Tenon.Caching.Interceptor.Castle;

/// <summary>
/// 缓存失效失败队列：延时双删失败时入队，供后续补偿重试。
/// </summary>
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

    /// <summary>
    /// 单例实例。
    /// </summary>
    public static CachingEvictFailedQueue Instance => Lazy.Value;

    /// <summary>
    /// 将删除失败的键数组入队。
    /// </summary>
    /// <param name="keys">待补偿删除的缓存键。</param>
    public void Enqueue(string[] keys)
    {
        if (keys?.Any() ?? false)
            FailedQueue.Enqueue(keys);


    }

    /// <summary>
    /// 尝试出队一批待重试的键。
    /// </summary>
    /// <param name="keys">出队的键数组，无数据时为 null。</param>
    /// <returns>是否成功出队。</returns>
    public bool TryDequeue(out string[]? keys)
    {
        return FailedQueue.TryDequeue(out keys);
    }
}