namespace Tenon.Infra.Windows.Win32.Models;

/// <summary>
///     表示窗口大小和位置标志，用于控制窗口的外观。
/// </summary>
public enum SetWindowPosFlags : uint
{
    /// <summary>
    ///     如果调用线程和拥有窗口的线程附属于不同的输入队列，则系统将请求发布到拥有窗口的线程。这样可以防止调用线程在其他线程处理请求时阻塞其执行。
    /// </summary>
    SwpAsyncWindowPos = 0x00004000,

    /// <summary>
    ///     阻止生成 WM_SYNCPAINT 消息。
    /// </summary>
    SwpDeferErase = 0x00002000,

    /// <summary>
    ///     在窗口周围绘制一个框架（在窗口类描述中定义）。
    /// </summary>
    SwpDrawFrame = 0x00000020,

    /// <summary>
    ///     发送 WM_NCCALCSIZE 消息给窗口，即使窗口的大小没有改变。如果未指定此标志，则仅在窗口的大小发生改变时发送 WM_NCCALCSIZE。
    /// </summary>
    SwpFrameChanged = 0x00000020,

    /// <summary>
    ///     隐藏窗口。
    /// </summary>
    SwpHideWindow = 0x00000080,

    /// <summary>
    ///     不激活窗口。如果未设置此标志，则激活窗口并将其移动到最上面的或非最上面的组（取决于 hWndInsertAfter 参数的设置）。
    /// </summary>
    SwpNoActivate = 0x00000010,

    /// <summary>
    ///     丢弃客户区的全部内容。如果未指定此标志，则保存客户区的有效内容，并在调整窗口大小或重新定位后将其复制回客户区。
    /// </summary>
    SwpNoCopyBits = 0x00000100,

    /// <summary>
    ///     不改变窗口的位置。
    /// </summary>
    SwpNoMove = 0x00000002,

    /// <summary>
    ///     不改变所有者窗口在 Z 顺序中的位置。
    /// </summary>
    SwpNoOwnerZOrder = 0x00000200,

    /// <summary>
    ///     不重新绘制更改。如果设置了此标志，则不会发生任何重绘。这适用于客户区、非客户区（包括标题栏和滚动条）以及因移动窗口而暴露的父窗口的任何部分。当设置了此标志时，应用程序必须显式地使窗口和父窗口需要重绘的任何部分失效或重绘。
    /// </summary>
    SwpNoRedraw = 0x00000008,

    /// <summary>
    ///     与 SWP_NOOWNERZORDER 标志相同。
    /// </summary>
    SwpNoReposition = 0x00000200,

    /// <summary>
    ///     抑制对窗口 Z 顺序的所有更改。
    /// </summary>
    SwpNoSendChanging = 0x00000400,

    /// <summary>
    ///     保留当前大小（忽略 cx 和 cy 参数）。
    /// </summary>
    SwpNoSize = 0x00000001,

    /// <summary>
    ///     保留当前 Z 顺序（忽略 hWndInsertAfter 参数）。
    /// </summary>
    SwpNoZOrder = 0x00000004,

    /// <summary>
    ///     显示窗口。
    /// </summary>
    SwpShowWindow = 0x00000040
}