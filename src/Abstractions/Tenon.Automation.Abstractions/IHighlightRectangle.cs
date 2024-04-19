using System.Drawing;

namespace Tenon.Automation.Abstractions;

public interface IHighlightRectangle
{
    public void SetLocation(Rectangle rectangle);

    public void Show();

    public void Hide();

    public void Close();
}