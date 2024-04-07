using Tenon.AspNetCore.Filters;

namespace CleanArchitecture.Identity.Api.Filters
{
    public sealed class CustomExceptionFilter(IWebHostEnvironment env, ILogger<GlobalExceptionsFilter> logger) : GlobalExceptionsFilter(env, logger)
    {
    }
}
