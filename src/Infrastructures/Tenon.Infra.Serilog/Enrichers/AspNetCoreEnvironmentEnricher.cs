using Serilog.Core;
using Serilog.Events;

namespace Tenon.Infra.Serilog.Enrichers;

internal class AspNetCoreEnvironmentEnricher : ILogEventEnricher
{
    private static readonly string AspNetEnvironment;

    static AspNetCoreEnvironmentEnricher()
    {
        AspNetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }

    public void Enrich(
        LogEvent logEvent,
        ILogEventPropertyFactory propertyFactory)
    {
        var enrichProperty = propertyFactory
            .CreateProperty(
                "AspNetCoreEnvironment",
                AspNetEnvironment);

        logEvent.AddOrUpdateProperty(enrichProperty);
    }
}