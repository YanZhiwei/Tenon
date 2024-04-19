using System.Collections.Concurrent;
using Tenon.Infra.Windows.Win32.Hooks;

namespace Tenon.Automation.Windows;

public class WindowsHighlightBehavior
{
    protected static readonly ManualResetEvent Mre = new(true);
    protected readonly ConcurrentStack<MouseEventArgs> MouseMoveQueue = new();
    protected readonly Thread WorkerThread;

    public WindowsHighlightBehavior()
    {
        WorkerThread = new Thread(ThreadProcedure)
        {
            Priority = ThreadPriority.AboveNormal,
            IsBackground = true
        };
        WorkerThread.SetApartmentState(ApartmentState.STA);
    }


    public event EventHandler<MouseEventArgs> IdentifyFromPointHandler;

    private void ThreadProcedure(object? obj)
    {
        var currentPosition = Cursor.Position;
        MouseMoveQueue.Push(
            new MouseEventArgs(MouseButtons.None, 0, currentPosition.X, currentPosition.Y, 0));
        while (true)
        {
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
    }


    public virtual void Stop()
    {
        MouseMoveQueue.Clear();
        Mre.Close();
        WorkerThread.Interrupt();
        MouseHook.MouseMove -= Hook_MouseMove;
        KeyboardHook.KeyDown -= Hook_KeyDown;
        KeyboardHook.KeyUp -= Hook_KeyUp;
        MouseHook.Uninstall();
        KeyboardHook.Uninstall();
    }

    public virtual void Suspend()
    {
        Mre.Reset();
    }

    public virtual void Resume()
    {
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
        MouseHook.Install();
        KeyboardHook.Install();
        MouseHook.MouseMove += Hook_MouseMove;
        KeyboardHook.KeyDown += Hook_KeyDown;
        KeyboardHook.KeyUp += Hook_KeyUp;
        WorkerThread.Start();
    }
}