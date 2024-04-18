namespace Tenon.Infra.Windows.Win32.Models;

/// <summary>
///     指定窗口如何显示的整数值。
/// </summary>
public enum ShowWindowCommand
{
    /// <summary>
    ///     隐藏窗口并激活其他窗口。
    /// </summary>
    Hide = 0,

    /// <summary>
    ///     激活并显示窗口。如果窗口最小化或最大化，Windows将其恢复到其原始大小和位置。
    /// </summary>
    ShowNormal = 1,

    /// <summary>
    ///     最小化指定的窗口并激活下一个顶层窗口。
    /// </summary>
    ShowMinimized = 2,

    /// <summary>
    ///     激活窗口并将其显示为最大化。
    /// </summary>
    ShowMaximized = 3,

    /// <summary>
    ///     显示窗口的状态，但不激活它。
    /// </summary>
    ShowNoActivate = 4,

    /// <summary>
    ///     激活窗口并显示它。
    /// </summary>
    Show = 5,

    /// <summary>
    ///     最小化指定的窗口并激活下一个顶层窗口。
    /// </summary>
    Minimize = 6,

    /// <summary>
    ///     激活窗口并显示它，但不改变其位置或大小。
    /// </summary>
    ShowNa = 8,

    /// <summary>
    ///     激活并显示窗口。如果窗口最小化或最大化，Windows将其恢复到其原始大小和位置。
    /// </summary>
    Restore = 9,

    /// <summary>
    ///     设置窗口的显示状态为由创建窗口时指定的值。
    /// </summary>
    ShowDefault = 10,

    /// <summary>
    ///     最小化指定的窗口并激活下一个顶层窗口。
    /// </summary>
    ForceMinimize = 11
}