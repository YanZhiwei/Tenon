using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tenon.Extensions.System;

public static class ProcessExtension
{
    public static Task WaitForExitAsync(this Process process,
        CancellationToken cancellationToken = default)
    {
        if (process.HasExited) return Task.CompletedTask;

        var tcs = new TaskCompletionSource<object>();
        process.EnableRaisingEvents = true;
        process.Exited += (sender, args) => tcs.TrySetResult(null);
        if (cancellationToken != default)
            cancellationToken.Register(() => tcs.SetCanceled(cancellationToken));

        return process.HasExited ? Task.CompletedTask : tcs.Task;
    }
}