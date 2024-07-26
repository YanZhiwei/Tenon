using Tenon.Infra.Windows.Win32;

namespace Tenon.Infra.Windows.Win32Tests;

[TestClass]
public class DisplayMonitorTests
{
    [TestMethod]
    public void FromPointTest()
    {
        Point point = new Point(800, 500);
        var actual = DisplayMonitor.FromPoint(point);
        Assert.IsNotNull(actual);
    }

    [TestMethod]
    public void GetDisplayDevicesTest()
    {
        var actual = DisplayMonitor.GetDisplayDevices();
        Assert.IsNotNull(actual);
        foreach (var item in actual) Assert.IsNotNull(item);
    }
}