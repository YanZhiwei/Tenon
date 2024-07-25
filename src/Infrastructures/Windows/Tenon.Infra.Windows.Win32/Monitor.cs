using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;
using CsWin32 = Windows.Win32;

namespace Tenon.Infra.Windows.Win32;

public class Monitor
{
    /// <summary>
    ///     获取当前显示器的缩放比例, 默认96
    ///     Windows 8.1 及更高版本
    /// </summary>
    /// <param name="hMonitor"></param>
    /// <param name="dpi"></param>
    /// <returns></returns>
    public static float GetScalingFactor(IntPtr hMonitor, float dpi = 96.0f)
    {
        int result = CsWin32.PInvoke.GetDpiForMonitor(new HMONITOR(hMonitor), MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI,
            out var dpiX, out var _);

        if (result != 0)
            return 1.0f;


        return dpiX / 96.0f;
    }
}