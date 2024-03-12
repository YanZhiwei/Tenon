using System.Reflection;
using Castle.DynamicProxy;
using Tenon.Infra.Castle.Attributes;
using Tenon.Infra.Castle.Models;

namespace Tenon.Infra.Castle.Interceptors;

public abstract class InterceptorBase : IAsyncInterceptor
{
    public void InterceptSynchronous(IInvocation invocation)
    {
        var attribute = GetAttribute(invocation);
        if (attribute == null)
        {
            invocation.Proceed();
            return;
        }

        Exception? ex = null;
        var metadata = GetInterceptMetadata(invocation);
        try
        {
            invocation.Proceed();
        }
        catch (Exception e)
        {
            ex = e;
            throw;
        }
        finally
        {
            Intercepted(attribute, metadata, ex);
        }
    }

    public void InterceptAsynchronous(IInvocation invocation)
    {
        var attribute = GetAttribute(invocation);
        if (attribute == null)
        {
            invocation.Proceed();
            invocation.ReturnValue = (Task)invocation.ReturnValue;
            return;
        }

        Exception? ex = null;
        var metadata = GetInterceptMetadata(invocation);
        try
        {
            invocation.Proceed();
        }
        catch (Exception e)
        {
            ex = e;
            throw;
        }
        finally
        {
            invocation.ReturnValue = Task.Factory.StartNew(async () =>
            {
                await InterceptedAsync(attribute, metadata, ex);
                await (Task)invocation.ReturnValue;
            });
        }
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        var attribute = GetAttribute(invocation);
        if (attribute == null)
        {
            invocation.Proceed();
            invocation.ReturnValue = (Task<TResult>)invocation.ReturnValue;
            return;
        }

        Exception? ex = null;
        var metadata = GetInterceptMetadata(invocation);
        try
        {
            invocation.Proceed();
        }
        catch (Exception e)
        {
            ex = e;
            throw;
        }
        finally
        {
            invocation.ReturnValue = Task.Factory.StartNew(async () =>
            {
                await InterceptedAsync(attribute, metadata, ex);
                var result = await (Task<TResult>)invocation.ReturnValue;
                return result;
            });
        }
    }

    private InterceptMetadata GetInterceptMetadata(IInvocation invocation)
    {
        var methodInfo = invocation.Method ?? invocation.MethodInvocationTarget;
        var metadata = new InterceptMetadata
        {
            MethodName = methodInfo.Name,
            ClassName = methodInfo.DeclaringType?.FullName ?? string.Empty,
            Arguments = invocation.Arguments
        };
        return metadata;
    }

    protected abstract Task InterceptedAsync(InterceptAttribute attribute,
        InterceptMetadata metadata, Exception? ex);

    protected abstract void Intercepted(InterceptAttribute attribute, InterceptMetadata metadata,
        Exception? ex);

    private InterceptAttribute? GetAttribute(IInvocation invocation)
    {
        var methodInfo = invocation.Method ?? invocation.MethodInvocationTarget;
        var attribute = methodInfo.GetCustomAttribute<InterceptAttribute>();
        return attribute;
    }
}