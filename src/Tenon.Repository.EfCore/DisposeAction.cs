namespace Tenon.Repository.EfCore;

/// <summary>
/// 用于在 using 语句结束时执行回调操作的辅助类
/// </summary>
internal sealed class DisposeAction : IDisposable
{
    private readonly Action _action;
    private bool _disposed;

    public DisposeAction(Action action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _action();
        _disposed = true;
    }
}
