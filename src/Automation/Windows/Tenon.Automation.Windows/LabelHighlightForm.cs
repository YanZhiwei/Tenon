using Tenon.Automation.Windows.Models;
using Tenon.Infra.Windows.Win32;
using Tenon.Infra.Windows.Win32.Models;
using SysForm = System.Windows.Forms.Form;

namespace Tenon.Automation.Windows;

public class LabelHighlightForm : SysForm
{
    private Label lblContent;

    public LabelHighlightForm(WindowProperties? properties = null)
    {
        InitializeComponent();
        properties = properties ?? new WindowProperties();
        FormBorderStyle = properties.FormBorderStyle;
        ShowInTaskbar = properties.ShowInTaskbar;
        TopMost = properties.TopMost;
        Visible = properties.Visible;
        Left = properties.Left;
        Top = properties.Top;
        BackColor = Color.Black;
        TransparencyKey = BackColor;
        SetFormStyle();
    }

    private void SetFormStyle()
    {
        var style = Window.GetLong(Handle, WindowLongPtrIndex.ExStyle);
        if (style != IntPtr.Zero)
            Window.SetLong(Handle, WindowLongPtrIndex.ExStyle, (int)style | 0x00000080);
    }

    public void SetLocation(Rectangle rectangle, string? content = null)
    {
        if (rectangle.IsEmpty) return;
        BeginInvoke(() =>
        {
            Window.SetPos(Handle, rectangle.X, rectangle.Y, this.Width, rectangle.Height,
                SpecialWindowHandles.Topmost, SetWindowPosFlags.SwpNoActivate);
            lblContent.Text = content;
        });
    }

    private void InitializeComponent()
    {
        lblContent = new Label();
        SuspendLayout();
        // 
        // lblContent
        // 
        lblContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblContent.AutoSize = true;
        lblContent.ForeColor = Color.Red;
        lblContent.Location = new Point(0, 0);
        lblContent.Name = "lblContent";
        lblContent.Size = new Size(0, 17);
        lblContent.TabIndex = 0;
        lblContent.TextAlign = ContentAlignment.BottomLeft;
        // 
        // LabelHighlightForm
        // 
        ClientSize = new Size(284, 261);
        Controls.Add(lblContent);
        Name = "LabelHighlightForm";
        ResumeLayout(false);
        PerformLayout();
    }

    public new void Show()
    {
        Window.Show(Handle, ShowWindowCommand.ShowNa);
    }
}