using Serilog.Core;
using Serilog.Events;

namespace Tenon.Infra.Serilog.Enrichers;

internal sealed class ThreadIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "ThreadId", Thread.CurrentThread.ManagedThreadId.ToString("D4")));
    }
}