using CsWin32 = Windows.Win32;

namespace Tenon.Infra.Windows.Win32;

public sealed class Process
{
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
}