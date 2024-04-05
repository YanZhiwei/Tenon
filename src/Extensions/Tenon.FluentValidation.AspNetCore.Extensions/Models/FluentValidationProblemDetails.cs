using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Tenon.FluentValidation.AspNetCore.Extensions.Models;

public class FluentValidationProblemDetails : HttpValidationProblemDetails
{
    public FluentValidationProblemDetails(IEnumerable<ValidationFailure> error)
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));
        Errors = error.Select(c => new ValidationError
        {
            PropertyName = c.PropertyName,
            ErrorMessage = c.ErrorMessage
        }).ToArray();
    }

    public new IEnumerable<ValidationError> Errors { get; }
}