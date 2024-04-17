namespace Tenon.Infra.Windows.Win32.Models;

public struct Rect
{
    public int Width { get; set; }

    public int Height { get; set; }

    public bool IsEmpty { get; set; }

    public int X { get; set; }

    public int Y { get; set; }
}