namespace MultiTenantSample.Extensions;

/// <summary>
/// 用于管理资源释放的操作类
/// </summary>
internal class DisposeAction : IDisposable
{
    private readonly Action _action;
    private bool _disposed;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="action">释放时执行的操作</param>
    public DisposeAction(Action action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _action();
        _disposed = true;
    }
}
