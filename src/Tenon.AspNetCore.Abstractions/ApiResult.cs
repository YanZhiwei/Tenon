using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Tenon.AspNetCore.Abstractions;

[Serializable]
public sealed class ApiResult
{
    public ApiResult(ProblemDetails problemDetails)
    {
        ProblemDetails = problemDetails;
    }

    public ApiResult()
    {
    }

    /// <summary>
    ///     成功静态实例
    /// </summary>
    public static ApiResult Success { get; } = new();

    /// <summary>
    ///     时间戳（毫秒）
    /// </summary>
    public long Timestamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>
    ///     问题详情
    /// </summary>
    public ProblemDetails? ProblemDetails { get; set; }

    /// <summary>
    ///     是否成功
    /// </summary>
    public bool Succeeded => ProblemDetails == null;

    /// <summary>
    ///     隐式转换为 ApiResult
    /// </summary>
    public static implicit operator ApiResult(ProblemDetails problemDetails)
    {
        return new ApiResult
        {
            ProblemDetails = problemDetails
        };
    }

    /// <summary>
    ///     创建失败结果
    /// </summary>
    public static ApiResult Error(HttpStatusCode statusCode, string title, string? detail = null)
    {
        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail
        };
        return new ApiResult(problem);
    }

    /// <summary>
    ///     创建泛型失败结果
    /// </summary>
    public static ApiResult<T> Error<T>(HttpStatusCode statusCode, string title, string? detail = null)
    {
        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail
        };
        return new ApiResult<T>(problem);
    }

    /// <summary>
    ///     创建泛型成功结果
    /// </summary>
    public static ApiResult<T> SuccessResult<T>(T value)
    {
        return new ApiResult<T>(value);
    }

    /// <summary>
    ///     创建空结果
    /// </summary>
    public static ApiResult<T> Empty<T>()
    {
        return new ApiResult<T>();
    }

    /// <summary>
    ///     创建 NotFound 结果
    /// </summary>
    public static ApiResult NotFound(string title, string? detail = null)
    {
        return Error(HttpStatusCode.NotFound, title, detail);
    }

    /// <summary>
    ///     创建 BadRequest 结果
    /// </summary>
    public static ApiResult BadRequest(string title, string? detail = null)
    {
        return Error(HttpStatusCode.BadRequest, title, detail);
    }

    /// <summary>
    ///     创建 Conflict 结果
    /// </summary>
    public static ApiResult Conflict(string title, string? detail = null)
    {
        return Error(HttpStatusCode.Conflict, title, detail);
    }
}

[Serializable]
public sealed class ApiResult<T>
{
    public ApiResult()
    {
    }

    public ApiResult(T value)
    {
        Content = value;
    }

    public ApiResult(ProblemDetails problemDetails)
    {
        ProblemDetails = problemDetails;
    }

    /// <summary>
    ///     时间戳（毫秒）
    /// </summary>
    public long Timestamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>
    ///     是否成功
    /// </summary>
    public bool Succeeded => ProblemDetails == null && Content != null;

    /// <summary>
    ///     返回数据
    /// </summary>
    public T Content { get; set; } = default!;

    /// <summary>
    ///     问题详情
    /// </summary>
    public ProblemDetails? ProblemDetails { get; set; }

    /// <summary>
    ///     支持隐式转换
    /// </summary>
    public static implicit operator ApiResult<T>(ApiResult result)
    {
        return new ApiResult<T>
        {
            ProblemDetails = result.ProblemDetails
        };
    }

    public static implicit operator ApiResult<T>(ProblemDetails problemDetails)
    {
        return new ApiResult<T>(problemDetails);
    }

    public static implicit operator ApiResult<T>(T value)
    {
        return new ApiResult<T>(value);
    }
}