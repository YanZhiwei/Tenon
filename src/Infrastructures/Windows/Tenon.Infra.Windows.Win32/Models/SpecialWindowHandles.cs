namespace Tenon.Infra.Windows.Win32.Models;

/// <summary>
///     表示特殊窗口句柄，用于指定相对于其位置将设置窗口位置的窗口。
/// </summary>
public enum SpecialWindowHandles
{
    /// <summary>
    ///     将窗口置于顶部。
    /// </summary>
    Top = 0,

    /// <summary>
    ///     将窗口置于底部。
    /// </summary>
    Bottom = 1,

    /// <summary>
    ///     将窗口置于最顶层。
    /// </summary>
    Topmost = -1,

    /// <summary>
    ///     将窗口置于非最顶层。
    /// </summary>
    NoTopmost = -2
}