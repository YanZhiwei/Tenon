using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Tenon.AspNetCore.Extensions;

public static class MvcBuilderExtension
{
    public static IMvcBuilder ConfigureJsonOptions(this IMvcBuilder builder, params JsonConverter[] jsonConverters)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));
        return builder.AddJsonOptions(options =>
        {
            if (jsonConverters?.Any() ?? false)
                foreach (var converter in jsonConverters)
                    options.JsonSerializerOptions.Converters.Add(converter);
            options.JsonSerializerOptions.Encoder =
                JavaScriptEncoder.Create(new TextEncoderSettings(UnicodeRanges.All));
            options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    }

    public static void ConfigureInvalidModelStateResponse(this IMvcBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));
        builder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Type = $"https://httpstatuses.com/{StatusCodes.Status422UnprocessableEntity}",
                    Title = "One or more model validation errors occurred.",
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Detail = context.ModelState.GetValidationSummary(),
                    Instance = context.HttpContext.Request.Path
                };
                problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                return new UnprocessableEntityObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };
        });
    }
}