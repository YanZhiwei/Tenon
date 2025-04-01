using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Tenon.FluentValidation.AspNetCore.Extensions.Models;

public class FluentValidationProblemDetails : HttpValidationProblemDetails
{
    public FluentValidationProblemDetails(IEnumerable<ValidationFailure> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        ValidationErrors = errors.Select(c => new ValidationError
        {
            PropertyName = c.PropertyName,
            ErrorMessage = c.ErrorMessage
        }).ToArray();
    }

    public IEnumerable<ValidationError> ValidationErrors { get; }
}