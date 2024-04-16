using Tenon.Infra.Windows.ChromiumAccessibility;
using Tenon.Infra.Windows.ChromiumAccessibility.Options;

[TestClass]
public class ChromeAccessibilityTests
{
    private readonly ChromeAccessibility _chromeAccessibility = new();

    public ChromeAccessibilityTests()
    {
        _chromeAccessibility.Detect();
    }

    [TestMethod]
    public void DetectTest()
    {
        Assert.IsNotNull(_chromeAccessibility.InstallPath);
    }

    [TestMethod]
    public void GetMainWindowHandleTest()
    {
        Assert.AreNotEqual(_chromeAccessibility.GetMainWindowHandle(), IntPtr.Zero);
    }

    [TestMethod]
    public async Task LaunchAsyncTest()
    {
        await _chromeAccessibility.LaunchAsync(new LaunchOptions
        {
            Url = new Uri("https://www.google.com/"),
            WaitForPageLoad = true
        });
        Assert.AreNotEqual(_chromeAccessibility.GetMainWindowHandle(), IntPtr.Zero);
    }
}