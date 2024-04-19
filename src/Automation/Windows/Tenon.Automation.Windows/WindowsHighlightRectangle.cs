using System.Collections.Concurrent;
using Tenon.Automation.Abstractions;
using Tenon.Infra.Windows.Form.Common;

namespace Tenon.Automation.Windows;

public sealed class WindowsHighlightRectangle : IHighlightRectangle
{
    private readonly HighlightForm _bottomForm = new();
    private readonly int _highlightLineWidth = 3;
    private readonly HighlightForm _leftForm = new();

    private readonly ConcurrentStack<MouseEventArgs> _mouseMoveQueue = new();
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
        Show();
    }

    public void Show()
    {
        _leftForm.UIBeginThread(c => c.Visible = true);
        _topForm.UIBeginThread(c => c.Visible = true);
        _rightForm.UIBeginThread(c => c.Visible = true);
        _bottomForm.UIBeginThread(c => c.Visible = true);
    }

    public void Hide()
    {
        _leftForm.UIBeginThread(c => c.Visible = false);
        _topForm.UIBeginThread(c => c.Visible = false);
        _rightForm.UIBeginThread(c => c.Visible = false);
        _bottomForm.UIBeginThread(c => c.Visible = false);
    }

    public void Close()
    {
        _leftForm.UIBeginThread(c => c.Close());
        _topForm.UIBeginThread(c => c.Close());
        _rightForm.UIBeginThread(c => c.Close());
        _bottomForm.UIBeginThread(c => c.Close());
    }
}