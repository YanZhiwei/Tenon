using System;
using System.Reflection;

namespace Tenon.Extensions.System;

public static class ReflectionExtension
{
    public static bool IsReturnTask(this MethodInfo methodInfo)
    {
        if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
        var returnType = methodInfo.ReturnType.GetTypeInfo();
        return returnType.IsTaskWithResult();
    }
}