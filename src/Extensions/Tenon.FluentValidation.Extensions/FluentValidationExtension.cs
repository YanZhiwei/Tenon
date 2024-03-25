using FluentValidation;

namespace Tenon.FluentValidation.Extensions;

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