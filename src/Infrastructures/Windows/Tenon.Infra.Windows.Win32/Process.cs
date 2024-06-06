using Tenon.Infra.Windows.Win32.Extensions;
using CsWin32 = Windows.Win32;

namespace Tenon.Infra.Windows.Win32;

public sealed class Process
{
    private const string FrameWindow = "ApplicationFrameWindow";

    public static bool TrySetDpiAware(out Exception? ex)
    {
        ex = null;
        try
        {
            return CsWin32.PInvoke.SetProcessDPIAware();
        }
        catch (Exception e)
        {
            ex = e;
            return false;
        }
    }

    public static IEnumerable<IntPtr> FindCoreWindows(int processId)
    {
        var appWindows = new List<IntPtr>();
        for (var appWindow =
                 CsWin32.PInvoke.FindWindowEx(IntPtr.Zero.ToHWnd(), IntPtr.Zero.ToHWnd(), FrameWindow, null);
             appWindow != IntPtr.Zero;
             appWindow = CsWin32.PInvoke.FindWindowEx(IntPtr.Zero.ToHWnd(), appWindow, FrameWindow, null))
        {
            var coreWindow =
                CsWin32.PInvoke.FindWindowEx(appWindow, IntPtr.Zero.ToHWnd(), "Windows.UI.Core.CoreWindow", null);
            if (coreWindow == IntPtr.Zero) continue;
            var corePid = Window.GetProcessId(coreWindow);
            if (corePid == processId)
                appWindows.Add(appWindow);
        }

        return appWindows;
    }

    public static IntPtr FindFirstCoreWindows(int processId)
    {
        return FindCoreWindows(processId).FirstOrDefault();
    }
}