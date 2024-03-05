using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Tenon.AspNetCore.Extensions;

public static class ModelStateExtensions
{
    public static string GetValidationSummary(this ModelStateDictionary modelState)
    {
        if (modelState.IsValid) return string.Empty;

        var builder = new StringBuilder();

        foreach (var item in modelState)
        {
            var state = item.Value;
            var message = state.Errors.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.ErrorMessage))?.ErrorMessage;
            if (string.IsNullOrWhiteSpace(message))
                message = state.Errors.FirstOrDefault(o => o.Exception != null)?.Exception?.Message;

            if (string.IsNullOrWhiteSpace(message)) continue;

            if (builder.Length > 0)
                builder.Append(Environment.NewLine);
            builder.Append(message);
        }

        return builder.ToString();
    }
}