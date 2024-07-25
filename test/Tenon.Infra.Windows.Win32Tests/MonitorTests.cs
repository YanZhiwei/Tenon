using System.Reflection;
using Monitor = Tenon.Infra.Windows.Win32.Monitor;

namespace Tenon.Infra.Windows.Win32Tests;

[TestClass]
public class MonitorTests
{
    [TestMethod]
    public void GetScalingFactorTest()
    {
        var currentScreen = Screen.FromPoint(new Point(500, 500));
        var hmonitor =
            currentScreen.GetType().GetProperty("Handle", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(currentScreen) as IntPtr? ?? IntPtr.Zero;
        var scalingFactor = Monitor.GetScalingFactor(hmonitor);
        Assert.AreEqual(1.25f, scalingFactor);
    }
}