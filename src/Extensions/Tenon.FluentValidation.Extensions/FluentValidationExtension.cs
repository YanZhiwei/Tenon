using FluentValidation;

namespace Tenon.FluentValidation.Extensions;

/// <summary>
///     https://docs.fluentvalidation.net/en/latest/aspnet.html
/// </summary>
public static class FluentValidationExtension
{
    public static IRuleBuilderOptions<T, TProperty> Required<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        return ruleBuilder
            .NotNull()
            .NotEmpty()
            .WithMessage("{PropertyName} is required");
    }
}