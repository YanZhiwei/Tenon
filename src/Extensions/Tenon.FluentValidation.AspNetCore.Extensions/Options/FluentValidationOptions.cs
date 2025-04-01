using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

namespace Tenon.FluentValidation.AspNetCore.Extensions.Options;

/// <summary>
///     FluentValidation 配置选项
/// </summary>
public class FluentValidationOptions
{
    /// <summary>
    ///     是否禁用 ASP.NET Core 默认的模型验证响应
    /// </summary>
    [Required(ErrorMessage = "必须指定是否禁用默认模型验证响应")]
    public bool DisableDefaultModelValidation { get; set; } = true;

    /// <summary>
    ///     验证器生命周期
    /// </summary>
    [Required(ErrorMessage = "必须指定验证器生命周期")]
    public ServiceLifetime ValidatorLifetime { get; set; } = ServiceLifetime.Scoped;
}