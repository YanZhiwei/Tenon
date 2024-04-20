using System.Collections.Concurrent;
using Tenon.Infra.Windows.Win32.Hooks;

namespace Tenon.Automation.Windows;

public class WindowsHighlightBehavior
{
    protected static readonly ManualResetEvent Mre = new(true);
    private static readonly object SyncRoot = new();
    protected readonly ConcurrentStack<MouseEventArgs> MouseMoveQueue = new();
    protected Thread? WorkerThread;

    public event EventHandler<MouseEventArgs> IdentifyFromPointHandler;

    private void ThreadProcedure(object? obj)
    {
        var currentPosition = Cursor.Position;
        MouseMoveQueue.Push(
            new MouseEventArgs(MouseButtons.None, 0, currentPosition.X, currentPosition.Y, 0));
        while (true)
            try
            {
                Mre.WaitOne(); //等待信号
                if (MouseMoveQueue.TryPop(out var e))
                {
                    MouseMoveQueue.Clear();
                    if (e?.Location == null || e.Location.IsEmpty) continue;
                    IdentifyFromPointHandler?.Invoke(this, e);
                }
            }
            catch
            {
                // ignored
            }
    }


    public virtual void Stop()
    {
        lock (SyncRoot)
        {
            MouseMoveQueue.Clear();
            Mre.Close();
            WorkerThread?.Interrupt();
            WorkerThread = null;
            MouseHook.MouseMove -= Hook_MouseMove;
            KeyboardHook.KeyDown -= Hook_KeyDown;
            KeyboardHook.KeyUp -= Hook_KeyUp;
            MouseHook.Uninstall();
            KeyboardHook.Uninstall();
        }
    }

    public virtual void Suspend()
    {
        if (WorkerThread != null)
            Mre.Reset();
    }

    public virtual void Resume()
    {
        if (WorkerThread != null)
            Mre.Set();
    }

    private void Hook_KeyUp(object? sender, KeyEventArgs e)
    {
    }

    private void Hook_KeyDown(object? sender, KeyEventArgs e)
    {
    }

    private void Hook_MouseMove(object? sender, MouseEventArgs e)
    {
        MouseMoveQueue.Push(e);
    }

    public virtual void Start()
    {
        lock (SyncRoot)
        {
            MouseHook.Install();
            KeyboardHook.Install();
            MouseHook.MouseMove += Hook_MouseMove;
            KeyboardHook.KeyDown += Hook_KeyDown;
            KeyboardHook.KeyUp += Hook_KeyUp;
            if (WorkerThread == null)
            {
                WorkerThread = new Thread(ThreadProcedure)
                {
                    Priority = ThreadPriority.AboveNormal,
                    IsBackground = true
                };
                WorkerThread.SetApartmentState(ApartmentState.STA);
            }
            WorkerThread.Start();
        }
    }
}