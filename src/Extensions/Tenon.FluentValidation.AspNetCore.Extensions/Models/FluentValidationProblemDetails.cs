using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Tenon.FluentValidation.AspNetCore.Extensions.Models;

public class FluentValidationProblemDetails : ValidationProblemDetails
{
    public FluentValidationProblemDetails()
    {
    }


    public FluentValidationProblemDetails(IEnumerable<ValidationError> errors)
    {
        Errors = errors;
    }

    public FluentValidationProblemDetails(IEnumerable<ValidationFailure> error)
    {
        Errors = ConvertValidationFailureToValidationErrors(error);
    }

    public new IEnumerable<ValidationError> Errors { get; } = new List<ValidationError>();

    private List<ValidationError> ConvertValidationFailureToValidationErrors(IEnumerable<ValidationFailure> errors)
    {
        List<ValidationError> validationErrors = new();

        switch (errors.Count())
        {
            case 0:
                break;

            case 1:
                validationErrors.Add(new ValidationError
                {
                    Code = null,
                    Message = errors.ElementAtOrDefault(0).ErrorMessage
                });
                break;

            default:
                var errorMessage = string.Join(Environment.NewLine, errors.Select(e => e.ErrorMessage));
                validationErrors.Add(new ValidationError { Message = errorMessage });
                break;
        }


        return validationErrors;
    }
}