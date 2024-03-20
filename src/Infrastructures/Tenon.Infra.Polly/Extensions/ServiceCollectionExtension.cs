using Microsoft.Extensions.DependencyInjection;
using Polly;
using Tenon.Infra.Polly.Helper;

namespace Tenon.Infra.Polly.Extensions;

public static class ServiceCollectionExtension
{
    public static IHttpClientBuilder AddHttpPolicyHandlers(this IHttpClientBuilder builder,
        IEnumerable<IAsyncPolicy<HttpResponseMessage>>? policies = null)
    {
        policies ??= new List<IAsyncPolicy<HttpResponseMessage>>
        {
            PolicyHelper.AddHttpRetryPolicy(),
            PolicyHelper.AddHttpTimeoutPolicy(),
            PolicyHelper.AddHttpCircuitBreakerPolicy()
        };
        policies.ToList().ForEach(policy => builder.AddPolicyHandler(policy));
        return builder;
    }
}