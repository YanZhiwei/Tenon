using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Tenon.AspNetCore.Abstractions;

public abstract class ApiServiceBase(ILogger<ApiServiceBase> logger)
{
    /// <summary>
    ///     返回成功结果
    /// </summary>
    /// <returns>成功的 ApiResult</returns>
    protected static ApiResult Success()
    {
        return ApiResult.Success;
    }

    /// <summary>
    ///     返回成功结果（带数据）
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="data">返回数据</param>
    /// <returns>成功的 ApiResult</returns>
    protected static ApiResult<T> Success<T>(T data)
    {
        return ApiResult.SuccessResult(data);
    }

    /// <summary>
    ///     返回标准错误结果
    /// </summary>
    /// <param name="statusCode">HTTP 状态码</param>
    /// <param name="title">标题</param>
    /// <param name="detail">详情</param>
    /// <param name="traceId">追踪 ID</param>
    /// <param name="errorType">错误类型</param>
    /// <param name="extensions">扩展属性</param>
    /// <returns>ApiResult</returns>
    protected ApiResult Error(
        HttpStatusCode statusCode,
        string title,
        string? detail = null,
        string? traceId = null,
        string? errorType = null,
        IDictionary<string, object>? extensions = null)
    {
        var problem = CreateProblemDetails(statusCode, title, detail, traceId, errorType, extensions);
        return ApiResult.Error(statusCode, title, detail);
    }

    /// <summary>
    ///     创建 ProblemDetails
    /// </summary>
    private ProblemDetails CreateProblemDetails(
        HttpStatusCode statusCode,
        string title,
        string? detail = null,
        string? traceId = null,
        string? errorType = null,
        IDictionary<string, object>? extensions = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = traceId,
            Type = errorType
        };

        if (extensions != null)
            foreach (var kvp in extensions)
                problemDetails.Extensions[kvp.Key] = kvp.Value;

        return problemDetails;
    }

    /// <summary>
    ///     常用标准错误返回方法
    /// </summary>
    protected ApiResult BadRequest(string title, string? detail = null)
    {
        return Error(HttpStatusCode.BadRequest, title, detail);
    }

    protected ApiResult NotFound(string title, string? detail = null)
    {
        return Error(HttpStatusCode.NotFound, title, detail);
    }

    protected ApiResult Conflict(string title, string? detail = null)
    {
        return Error(HttpStatusCode.Conflict, title, detail);
    }

    protected ApiResult Unauthorized(string title = "Unauthorized", string? detail = null)
    {
        return Error(HttpStatusCode.Unauthorized, title, detail);
    }

    /// <summary>
    ///     安全执行异步方法
    /// </summary>
    protected async Task<ApiResult> ExecuteSafeAsync(
        Func<Task> action,
        string errorMessage = "An error occurred",
        HttpStatusCode errorStatusCode = HttpStatusCode.InternalServerError)
    {
        try
        {
            await action();
            return Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, errorMessage);
            return Error(errorStatusCode, errorMessage, ex.Message);
        }
    }

    /// <summary>
    ///     安全执行异步方法（带返回值）
    /// </summary>
    protected async Task<ApiResult<T>> ExecuteSafeAsync<T>(
        Func<Task<T>> action,
        string errorMessage = "An error occurred",
        HttpStatusCode errorStatusCode = HttpStatusCode.InternalServerError)
    {
        try
        {
            var result = await action();
            return Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, errorMessage);
            return Error(errorStatusCode, errorMessage, ex.Message);
        }
    }
}