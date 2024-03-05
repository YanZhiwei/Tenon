using System;
using System.Text;

namespace Tenon.Extensions.Exception;

public static class ExceptionExtension
{
    public static string GetExceptionDetail(this global::System.Exception exception)
    {
        var detail = new StringBuilder();
        detail.AppendLine("***************************************");
        detail.AppendLine($"Occurring Time： {DateTime.UtcNow}");
        detail.AppendLine($"HResult： {exception.HResult} ");
        detail.AppendLine($"InnerException： {exception.InnerException} ");
        detail.AppendLine($"Source： {exception.Source} ");
        detail.AppendLine($"TargetSite： {exception.TargetSite}");
        detail.AppendLine($"StackTrace： {exception.StackTrace}");
        detail.AppendLine($"Message： {exception.Message} ");
        detail.AppendLine("***************************************");
        return detail.ToString();
    }
}