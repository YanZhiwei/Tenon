using Tenon.Infra.Castle.Attributes;
using Tenon.Infra.Castle.Interceptors;
using Tenon.Infra.Castle.Models;

namespace CastleInterceptSample;

public class OperateLogAsyncInterceptor : InterceptorBase
{
    protected override Task InterceptedAsync(InterceptAttribute attribute, InterceptMetadata metadata,
        Exception? ex)
    {
        Console.WriteLine("记录日志：");
        return Task.CompletedTask;
    }

    protected override void Intercepted(InterceptAttribute attribute, InterceptMetadata metadata,
        Exception? ex)
    {
        if (attribute is OperateLogAttribute)
            Console.WriteLine("记录日志.");
    }

}