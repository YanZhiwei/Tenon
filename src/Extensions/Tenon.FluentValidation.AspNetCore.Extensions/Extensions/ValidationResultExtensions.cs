using System.Diagnostics;
using System.Text.Json.Serialization;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Tenon.FluentValidation.AspNetCore.Extensions.Extensions;

/// <summary>
///     验证结果扩展方法
/// </summary>
public static class ValidationResultExtensions
{
    /// <summary>
    ///     将验证结果转换为本地化的 ValidationProblemDetails
    /// </summary>
    /// <param name="validationResult">验证结果</param>
    /// <param name="localizer">字符串本地化器</param>
    /// <param name="httpContextAccessor">HTTP 上下文访问器</param>
    /// <returns>验证问题详情</returns>
    public static ActionResult ToLocalizedProblemDetails(
        this ValidationResult validationResult,
        IStringLocalizer localizer,
        IHttpContextAccessor httpContextAccessor)
    {
        if (validationResult.IsValid)
            return new OkResult();

        var validationErrors = validationResult.Errors
            .Select(error =>
            {
                var localizedString = localizer[error.ErrorMessage];
                var localizedMessage = localizedString.ResourceNotFound ? error.ErrorMessage : localizedString.Value;

                if (error.FormattedMessagePlaceholderValues != null)
                {
                    var formattedArgs = error.FormattedMessagePlaceholderValues
                        .Where(kv => kv.Key != "PropertyName" && kv.Key != "PropertyValue")
                        .OrderBy(kv => kv.Key)
                        .Where(kv => kv.Value != null)
                        .Select(kv => kv.Value)
                        .ToArray();

                    if (formattedArgs.Length > 0)
                        try
                        {
                            localizedMessage = string.Format(localizedMessage, formattedArgs);
                        }
                        catch (FormatException ex)
                        {
                            Debug.WriteLine($"Format error: {ex.Message}");
                        }
                }

                return new ValidationError
                {
                    Field = error.PropertyName,
                    Message = localizedMessage,
                    Code = error.ErrorCode ?? "ValidationError",
                    AttemptedValue = error.AttemptedValue?.ToString()
                };
            })
            .ToList();

        // 获取受影响的字段列表
        var affectedFields = validationErrors
            .Select(e => e.Field)
            .Distinct()
            .OrderBy(f => f)
            .ToList();

        var customProblemDetails = new CustomValidationProblemDetails
        {
            Instance = httpContextAccessor?.HttpContext?.Request?.Path.Value,
            Errors = validationErrors
        };

        // 设置详细信息
        customProblemDetails.Title = localizer["Validation_Failed"];
        customProblemDetails.Detail = string.Format(
            localizer["Validation_Summary"],
            validationErrors.Count,
            string.Join(", ", affectedFields));

        return new BadRequestObjectResult(customProblemDetails);
    }
}

/// <summary>
///     自定义验证错误详情
/// </summary>
public class CustomValidationProblemDetails : ProblemDetails
{
    public CustomValidationProblemDetails()
    {
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        Status = StatusCodes.Status400BadRequest;
    }

    /// <summary>
    ///     错误列表
    /// </summary>
    [JsonPropertyName("errors")]
    public List<ValidationError> Errors { get; set; }
}

/// <summary>
///     验证错误信息
/// </summary>
public class ValidationError
{
    /// <summary>
    ///     字段名
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; }

    /// <summary>
    ///     错误消息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    ///     错误代码
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; }

    /// <summary>
    ///     用户输入的值
    /// </summary>
    [JsonPropertyName("attemptedValue")]
    public string AttemptedValue { get; set; }
}