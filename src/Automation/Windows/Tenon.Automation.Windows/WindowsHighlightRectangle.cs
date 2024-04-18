using Tenon.Automation.Abstractions;

namespace Tenon.Automation.Windows;

public sealed class WindowsHighlightRectangle : IHighlightRectangle
{
    private readonly HighlightForm _bottomForm = new();
    private readonly int _highlightLineWidth = 3;
    private readonly HighlightForm _leftForm = new();
    private readonly HighlightForm _rightForm = new();
    private readonly HighlightForm _topForm = new();

    public void SetLocation(Rectangle location)
    {
        _leftForm.SetLocation(new Rectangle
        {
            X = location.Left - _highlightLineWidth,
            Y = location.Top,
            Width = _highlightLineWidth,
            Height = location.Height
        });
        _topForm.SetLocation(new Rectangle
        {
            X = location.Left - _highlightLineWidth,
            Y = location.Top - _highlightLineWidth,
            Width = location.Width + 2 * _highlightLineWidth,
            Height = _highlightLineWidth
        });
        _rightForm.SetLocation(new Rectangle
        {
            X = location.Left + location.Width,
            Y = location.Top,
            Width = _highlightLineWidth,
            Height = location.Height
        });
        _bottomForm.SetLocation(new Rectangle
        {
            X = location.Left - _highlightLineWidth,
            Y = location.Top + location.Height,
            Width = location.Width + 2 * _highlightLineWidth,
            Height = _highlightLineWidth
        });
    }

    public void Show()
    {
        throw new NotImplementedException();
    }

    public void Hide()
    {
        throw new NotImplementedException();
    }
}