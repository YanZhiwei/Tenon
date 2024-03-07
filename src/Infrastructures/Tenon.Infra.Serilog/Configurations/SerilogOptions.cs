namespace Tenon.Infra.Serilog.Configurations;

public sealed class SerilogOptions
{
    public bool IsClearProviders { get; set; }

    public string Application { get; set; }
}