using Win32 = Tenon.Infra.Windows.Win32;
using SysProcess = System.Diagnostics.Process;

namespace Tenon.Windows.Extensions;

public static class ProcessExtension
{
    public static IEnumerable<IntPtr> FindCoreWindows(this SysProcess process)
    {
        return Win32.Process.FindCoreWindows(process.Id);
    }

    public static IntPtr FindFirstCoreWindows(this SysProcess process)
    {
        return Win32.Process.FindFirstCoreWindows(process.Id);
    }
}