using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Tenon.Helper.Internal
{
    public static class ProcessHelper
    {
        public static bool IsRunning(string path)
        {
            Checker.Begin()
                .NotNullOrEmpty(path, nameof(path))
                .CheckFileExists(path);
            var processName = Path.GetFileNameWithoutExtension(path);
            var processes = Process.GetProcessesByName(processName);
            return (bool)processes?.Any();
        }

        public static bool IsRunning(int id)
        {
            return Process.GetProcesses().Any(x => x.Id == id);
        }

        public static Process GetProcessById(int id)
        {
            var processes = Process.GetProcesses();
            return processes.FirstOrDefault(pr => pr.Id == id);
        }

        public static void RunCmd(string cmdText, bool runAsAdmin = true, bool useShellExecute = false)
        {
            Checker.Begin().NotNullOrEmpty(cmdText, nameof(cmdText));
            using (var process = new Process())
            {
                var startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/c {cmdText}",
                    Verb = runAsAdmin ? "runas" : string.Empty,
                    UseShellExecute = useShellExecute
                };
                process.StartInfo = startInfo;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }

        public static void RunCmd(string cmdText, out string succeedLog, out string failedLog, bool runAsAdmin = true,
            bool useShellExecute = false)
        {
            Checker.Begin().NotNullOrEmpty(cmdText, nameof(cmdText));
            var succeedOutput = new StringBuilder();
            var failedOutput = new StringBuilder();
            using (var process = new Process())
            {
                var startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/c {cmdText}",
                    Verb = runAsAdmin ? "runas" : string.Empty,
                    UseShellExecute = useShellExecute,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                process.OutputDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        succeedOutput.AppendFormat(e.Data);
                };
                process.ErrorDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        failedOutput.AppendFormat(e.Data.ToString());
                };
                process.EnableRaisingEvents = true;
                process.StartInfo = startInfo;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }

            succeedLog = succeedOutput.ToString();
            failedLog = failedOutput.ToString();
        }


        public static FileVersionInfo GetCurrentVersion()
        {
            var fileName = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
            return string.IsNullOrEmpty(fileName) ? null : FileVersionInfo.GetVersionInfo(fileName);
        }
    }
}