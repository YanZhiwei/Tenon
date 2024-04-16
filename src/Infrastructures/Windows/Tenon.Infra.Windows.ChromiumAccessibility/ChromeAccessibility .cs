using System.Diagnostics;
using Microsoft.Win32;
using Tenon.Infra.Windows.ChromiumAccessibility.Options;

namespace Tenon.Infra.Windows.ChromiumAccessibility;

public sealed class ChromeAccessibility
{
    public string? InstallPath { get; private set; }
    public bool HasInstalled => !string.IsNullOrEmpty(InstallPath);

    public void Detect()
    {
        InstallPath = GetInstallPath();
    }

    public IntPtr GetMainWindowHandle()
    {
        var chromeProcesses = Process.GetProcessesByName("chrome");
        foreach (var chromeProcess in chromeProcesses)
            if (chromeProcess.MainWindowHandle != IntPtr.Zero)
                return chromeProcess.MainWindowHandle;

        return IntPtr.Zero;
    }

    public async Task LaunchAsync(LaunchOptions? option = null, CancellationToken token = default)
    {
        try
        {
            option = option ?? new LaunchOptions();
            option.Args ??= [];
            var chromeProcess = new Process();
            chromeProcess.StartInfo.FileName = InstallPath;
            if (option.Url != null)
            {
                option.Args.Add(option.Url.ToString());
                option.Args.Add("--new-window");
            }

            if (option.Maximized)
                option.Args.Add("--start-maximized");
            if (option.Incognito)
                option.Args.Add("--incognito");

            foreach (var arg in option.Args)
                chromeProcess.StartInfo.Arguments += " " + arg;
            if (!string.IsNullOrEmpty(option.AppLocalData))
                chromeProcess.StartInfo.Environment["LOCALAPPDATA"] = option.AppLocalData;

            chromeProcess.Start();
            if (option.WaitForPageLoad)
            {
                chromeProcess.WaitForInputIdle();
                while (!chromeProcess.MainWindowTitle.Contains(" - Google Chrome"))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), token);
                    chromeProcess.Refresh();
                }
            }

            await chromeProcess.WaitForExitAsync(token);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private string? GetInstallPath()
    {
        var path = Registry.GetValue(@"HKEY_CLASSES_ROOT\ChromeHTML\shell\open\command", null, null) as string;
        if (path != null)
        {
            var split = path.Split('\"');
            path = split.Length >= 2 ? split[1] : null;
        }

        if (path != null) return path;
        const string suffix = @"Google\Chrome\Application\chrome.exe";
        var prefixes = new[] { Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) };
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        if (programFilesX86 != programFiles)
        {
            prefixes = (string[])prefixes.Concat(new[] { programFiles });
        }
        else
        {
            if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion",
                    "ProgramW6432Dir", null) is string programFilesDirFromReg)
                prefixes = (string[])prefixes.Concat(new[] { programFilesDirFromReg });
        }

        prefixes = (string[])prefixes.Concat(new[] { programFilesX86 });
        path = prefixes.Select(prefix => Path.Combine(prefix, suffix)).FirstOrDefault(File.Exists);
        return path;
    }

    public void SetMaximize()
    {
    }

    public void SetMinimize()
    {
    }
}