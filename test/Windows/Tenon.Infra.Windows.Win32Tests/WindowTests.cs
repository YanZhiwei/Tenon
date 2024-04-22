using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tenon.Automation.Windows;
using Tenon.Infra.Windows.ChromiumAccessibility;
using Tenon.Infra.Windows.Win32;
using Tenon.Infra.Windows.Win32.Models;

namespace Tenon.Infra.Windows.Win32.Tests
{
    [TestClass]
    public class WindowTests
    {

    }
}

namespace Tenon.Infra.Windows.Win32Tests
{
    [TestClass]
    public class WindowTests
    {
        private readonly ChromeAccessibility _chromeAccessibility = new();

        [TestMethod]
        public void GetRectTest()
        {
            var mainWindowHandle = _chromeAccessibility.GetMainWindowHandle();
            Assert.IsNotNull(mainWindowHandle);
            var actual = Window.GetRect(mainWindowHandle);
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GetRectangleTest()
        {
            var mainWindowHandle = _chromeAccessibility.GetMainWindowHandle();
            Assert.IsNotNull(mainWindowHandle);
            var actual = Window.GetRectangle(mainWindowHandle);
            Assert.IsNotNull(actual);
        }

        [TestMethod()]
        public void ShowTest()
        {
            var mainWindowHandle = _chromeAccessibility.GetMainWindowHandle();
            Assert.IsNotNull(mainWindowHandle);
            var actual = Window.Show(mainWindowHandle, ShowWindowCommand.ShowMaximized);
            Assert.IsTrue(actual);


        }

        [TestMethod()]
        public void GetLongTest()
        {
            var mainWindowHandle = _chromeAccessibility.GetMainWindowHandle();
            Assert.IsNotNull(mainWindowHandle);
            var actual = Window.GetLong(mainWindowHandle, WindowLongPtrIndex.ExStyle);
            Assert.IsNotNull(actual);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.Sleep(TimeSpan.FromSeconds(30));
        }
    }
}