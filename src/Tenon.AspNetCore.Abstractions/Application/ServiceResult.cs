using Microsoft.AspNetCore.Mvc;

namespace Tenon.AspNetCore.Abstractions.Application;

[Serializable]
public sealed class ServiceResult
{
    public ServiceResult(ProblemDetails problemDetails)
    {
        ProblemDetails = problemDetails;
    }

    public ServiceResult()
    {
    }

    public long Timestamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public ProblemDetails? ProblemDetails { get; set; }

    public bool Succeeded => ProblemDetails == null;

    public static implicit operator ServiceResult(ProblemDetails problemDetails)
    {
        return new ServiceResult
        {
            ProblemDetails = problemDetails
        };
    }
}

[Serializable]
public sealed class ServiceResult<TValue>
{
    public ServiceResult()
    {
    }

    public ServiceResult(TValue value)
    {
        Content = value;
    }

    public ServiceResult(ProblemDetails problemDetails)
    {
        ProblemDetails = problemDetails;
    }

    public long Timestamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public bool Succeeded => ProblemDetails == null && Content != null;

    public TValue Content { get; set; } = default!;

    public ProblemDetails? ProblemDetails { get; set; }

    public static implicit operator ServiceResult<TValue>(ServiceResult result)
    {
        return new ServiceResult<TValue>
        {
            ProblemDetails = result.ProblemDetails
        };
    }

    public static implicit operator ServiceResult<TValue>(ProblemDetails problemDetails)
    {
        return new ServiceResult<TValue>
        {
            ProblemDetails = problemDetails
        };
    }

    public static implicit operator ServiceResult<TValue>(TValue value)
    {
        return new ServiceResult<TValue>(value);
    }
}