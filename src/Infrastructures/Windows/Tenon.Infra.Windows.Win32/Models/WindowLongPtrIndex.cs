namespace Tenon.Infra.Windows.Win32.Models;

public enum WindowLongPtrIndex
{
    /// <summary>
    ///     扩展窗口样式
    /// </summary>
    ExStyle = -20,

    /// <summary>
    ///     应用程序实例句柄
    /// </summary>
    HInstance = -6,

    /// <summary>
    ///     标识
    /// </summary>
    Id = -12,

    /// <summary>
    ///     窗口样式
    /// </summary>
    Style = -16,

    /// <summary>
    ///     用户数据
    /// </summary>
    UserData = -21,

    /// <summary>
    ///     窗口过程地址或者是一个句柄
    /// </summary>
    WndProc = -4
}