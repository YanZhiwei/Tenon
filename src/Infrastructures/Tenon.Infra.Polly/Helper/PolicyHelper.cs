using Polly;
using Polly.Timeout;

namespace Tenon.Infra.Polly.Helper;

public sealed class PolicyHelper
{
    public static IAsyncPolicy<HttpResponseMessage> AddHttpRetryPolicy()
    {
        return Policy.Handle<TimeoutRejectedException>()
            .OrResult<HttpResponseMessage>(response => (int)response.StatusCode >= 500)
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(5)
            });
    }

    public static IAsyncPolicy<HttpResponseMessage> AddHttpTimeoutPolicy(int timeoutSec = 10)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(timeoutSec);
    }

    public static IAsyncPolicy<HttpResponseMessage> AddHttpCircuitBreakerPolicy()
    {
        var circuitBreakerPolicy = Policy.Handle<Exception>()
            .CircuitBreakerAsync
            (
                10,
                TimeSpan.FromMinutes(3),
                (ex, breakDelay) =>
                {
                    //todo
                },
                () =>
                {
                    //todo
                },
                () =>
                {
                    //todo
                }
            );
        return circuitBreakerPolicy.AsAsyncPolicy<HttpResponseMessage>()
    }
}