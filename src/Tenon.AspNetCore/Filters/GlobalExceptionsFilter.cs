using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tenon.Extensions.Exception;

namespace Tenon.AspNetCore.Filters;

public class GlobalExceptionsFilter(IWebHostEnvironment env, ILogger<GlobalExceptionsFilter> logger)
    : IExceptionFilter
{
    protected readonly IWebHostEnvironment Env = env ?? throw new ArgumentNullException(nameof(env));

    protected readonly ILogger<GlobalExceptionsFilter> Logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    protected bool IsDevEnvironment;

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var problemDetails = new ProblemDetails
        {
            Status = 500
        };
        var requestId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
        var eventId = new EventId(exception.HResult, requestId);
        var hostAndPort = context.HttpContext.Request.Host.HasValue
            ? context.HttpContext.Request.Host.Value
            : string.Empty;
        var requestUrl = string.Concat(hostAndPort, context.HttpContext.Request.Path);
        problemDetails.Instance = requestUrl;
        if (IsDevelopment())
        {
            IsDevEnvironment = true;
            problemDetails.Detail = exception.GetExceptionDetail();
            problemDetails.Title = exception.Message;
        }
        else
        {
            problemDetails.Detail = $"Server Error，Please contact the administrator, Event Id:{eventId}.";
            problemDetails.Title = "Server Error";
        }

        CustomProblemDetails(context, problemDetails, IsDevEnvironment);
        context.Result = new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
        context.ExceptionHandled = true;
    }

    protected virtual void CustomProblemDetails(ExceptionContext context, ProblemDetails problemDetails,
        bool isDevEnvironment)
    {
    }

    protected virtual bool IsDevelopment()
    {
        return Env.IsDevelopment();
    }
}