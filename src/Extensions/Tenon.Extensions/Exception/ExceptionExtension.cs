using System;
using System.Text;

namespace Tenon.Extensions.Exception;

public static class ExceptionExtension
{
    public static string GetExceptionDetail(this global::System.Exception exception)
    {
        var detail = new StringBuilder();
        detail.Append("***************************************");
        detail.AppendFormat($" Occurring Time： {DateTime.UtcNow} ");
        detail.AppendFormat($" HResult： {exception.HResult} ");
        detail.AppendFormat($" InnerException： {exception.InnerException} ", exception.InnerException);
        detail.AppendFormat($" Source： {exception.Source} ", exception.Source);
        detail.AppendFormat($" TargetSite： {exception.TargetSite} ", exception.TargetSite);
        detail.AppendFormat($" StackTrace： {exception.StackTrace} ", exception.StackTrace);
        detail.AppendFormat($" Message： {exception.Message} ", exception.Message);
        detail.Append("***************************************");
        return detail.ToString();
    }
}