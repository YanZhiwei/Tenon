using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Tenon.Infra.Windows.Win32.Models;

namespace Tenon.Infra.Windows.Win32;

public class DisplayMonitor
{
    private static readonly Lazy<IEnumerable<DisplayDevice>> Devices = new(GetDisplayDevices);

    public static DisplayDevice? GetPrimaryDisplay()
    {
        return Devices.Value.FirstOrDefault(c => c.IsPrimary);
    }

    public static IEnumerable<DisplayDevice> GetDisplayDevices()
    {
        uint deviceIndex = 0;
        var displayDevice = new DISPLAY_DEVICEW();
        displayDevice.cb = (uint)Marshal.SizeOf(displayDevice);

        while (PInvoke.EnumDisplayDevices(null, deviceIndex, ref displayDevice, 0))
        {
            var devMode = new DEVMODEW();
            devMode.dmSize = (ushort)Marshal.SizeOf(devMode);

            if (PInvoke.EnumDisplaySettings(displayDevice.DeviceName.ToString(),
                    ENUM_DISPLAY_SETTINGS_MODE.ENUM_CURRENT_SETTINGS, ref devMode))
            {
                var screen = Screen.AllScreens.FirstOrDefault(c => c.DeviceName == displayDevice.DeviceName.ToString());
                if (screen == null) continue;
                var display = new DisplayDevice
                {
                    Name = displayDevice.DeviceName.ToString(),
                    Description = displayDevice.DeviceString.ToString(),
                    RealResolution = new Size(screen.Bounds.Width, screen.Bounds.Height),
                    ColorDepth = devMode.dmBitsPerPel,
                    RefreshRate = devMode.dmDisplayFrequency,
                    IsPrimary = screen.Primary,
                    VirtualResolution = new Size((int)devMode.dmPelsWidth, (int)devMode.dmPelsHeight)
                };

                display.ScaleX = display.VirtualResolution.Width / (float)display.RealResolution.Width;
                display.ScaleY = display.VirtualResolution.Height / (float)display.RealResolution.Height;


                yield return display;
            }

            deviceIndex++;
            displayDevice = new DISPLAY_DEVICEW();
            displayDevice.cb = (uint)Marshal.SizeOf(displayDevice);
        }
    }

    public static DisplayDevice? FromPoint(Point point)
    {
        var screen = Screen.FromPoint(point);
        return Devices.Value.FirstOrDefault(c => c.Name == screen.DeviceName);
    }
}