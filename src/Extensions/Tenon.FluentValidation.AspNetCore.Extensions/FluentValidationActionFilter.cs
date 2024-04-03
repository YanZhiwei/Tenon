using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Tenon.FluentValidation.AspNetCore.Extensions;

public sealed class FluentValidationActionFilter(IValidator validatorFactory) : IAsyncActionFilter
{
    private readonly IValidator _validator =
        validatorFactory ?? throw new ArgumentNullException(nameof(IValidator));

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var parameters = context.ActionDescriptor.Parameters;

        foreach (var parameter in parameters)
        {
            var argument = context.ActionArguments.SingleOrDefault(a => a.Key == parameter.Name).Value;
            if (argument == null) continue;
            var validationResult = await _validator.ValidateAsync(new ValidationContext<object>(argument));

            if (!validationResult.IsValid)
            {
                context.Result = new BadRequestObjectResult(validationResult.Errors);
                return;
            }
        }

        await next();
    }
}