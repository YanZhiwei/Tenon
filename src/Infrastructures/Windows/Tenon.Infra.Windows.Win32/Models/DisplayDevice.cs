namespace Tenon.Infra.Windows.Win32.Models;

public sealed class DisplayDevice
{

    public bool IsPrimary { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }

    public Size RealResolution { get; set; }

    public Size VirtualResolution { get; set; }
    /// <summary>
    ///     Color Depth(bits)
    /// </summary>
    public uint ColorDepth { get; set; }

    /// <summary>
    ///     Refresh Rate(Hz)
    /// </summary>
    public uint RefreshRate { get; set; }

    public float ScaleY { get; set; }

    public float ScaleX { get; set; }
}