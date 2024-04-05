using System.Net;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tenon.FluentValidation.AspNetCore.Extensions.Models;

namespace Tenon.FluentValidation.AspNetCore.Extensions;

public static class FluentValidationExtension
{
    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
    {
        foreach (var error in result.Errors)
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    public static HttpValidationProblemDetails ToValidationProblemDetails(this ValidationResult validationResult)
    {
        if (validationResult == null)
            throw new ArgumentNullException(nameof(validationResult));


        var errors = validationResult.Errors ?? [];
        Dictionary<string, string[]> errorDictionary = new Dictionary<string, string[]>();

        foreach (var error in errors)
        {
            if (errorDictionary.ContainsKey(error.PropertyName))
            {
                errorDictionary[error.PropertyName] = errorDictionary[error.PropertyName].Append(error.ErrorMessage).ToArray();
            }
            else
            {
                errorDictionary.Add(error.PropertyName, new string[] { error.ErrorMessage });
            }
        }
        var problemDetails = new HttpValidationProblemDetails(errorDictionary)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = (int)HttpStatusCode.BadRequest
        };
        return problemDetails;
    }

    public static FluentValidationProblemDetails ToFluentValidationProblemDetails(this ValidationResult validationResult)
    {
        if (validationResult == null)
            throw new ArgumentNullException(nameof(validationResult));

        var problemDetails = new FluentValidationProblemDetails(validationResult.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = (int)HttpStatusCode.BadRequest
        };
        return problemDetails;
    }
}