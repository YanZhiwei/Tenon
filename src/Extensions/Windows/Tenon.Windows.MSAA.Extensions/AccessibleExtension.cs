using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Accessibility;

namespace Tenon.Windows.MSAA.Extensions;

public static class AccessibleExtension
{
    /// <summary>
    ///     将指定接口的地址检索到与给定窗口关联的对象
    ///     https://learn.microsoft.com/zh-cn/windows/win32/api/oleacc/nf-oleacc-accessibleobjectfromwindow
    /// </summary>
    /// <param name="hwnd">这是窗口的句柄，用于指定要检索对象的窗口。如果要检索光标或插入符对象的接口指针，请指定NULL，并在dwObjectId中使用适当的对象ID。</param>
    /// <param name="dwObjectId">对象的ID</param>
    /// <param name="refId">请求接口的引用标识符</param>
    /// <param name="ppvObject">接收指定接口地址的指针变量的地址</param>
    /// <returns></returns>
    [DllImport("Oleacc")]
    private static extern int AccessibleObjectFromWindow(IntPtr hwnd, int dwObjectId, ref Guid refId,
        ref IAccessible ppvObject);

    /// <summary>
    ///     检索对应于IAccess接口的特定实例的窗口句柄
    ///     https://learn.microsoft.com/zh-cn/windows/win32/api/oleacc/nf-oleacc-windowfromaccessibleobject
    /// </summary>
    /// <param name="accessible">指向将检索其相应窗口句柄的 IAccessible 接口的指针。</param>
    /// <param name="phwnd">接收包含 pacc 中指定的对象的窗口句柄的变量的地址。 </param>
    /// <returns>如果成功，则返回 S_OK。</returns>
    [DllImport("Oleacc")]
    private static extern int WindowFromAccessibleObject(IAccessible accessible, out IntPtr phwnd);

    /// <summary>
    ///     检索描述单个预定义状态位标志的对象状态的本地化字符串。
    ///     https://learn.microsoft.com/zh-cn/windows/win32/api/oleacc/nf-oleacc-getstatetexta
    /// </summary>
    /// <param name="dwStateBit">一个对象状态常量</param>
    /// <param name="lpszStateBit">接收状态文本字符串的缓冲区地址</param>
    /// <param name="cchStateBitMax">由LPSZSTATEBIT参数指向的缓冲器的大小</param>
    /// <returns>
    ///     如果成功，并且 lpszStateBit 为非 NULL，则返回值是 (ANSI 字符串) 字节数或复制到缓冲区 (Unicode 字符串) 的字符数，不包括以 null 结尾的字符。 如果 lpszStateBit 为
    ///     NULL，则返回值表示字符串的长度，不包括 null 字符。
    /// </returns>
    [DllImport("Oleacc")]
    private static extern int GetStateText(int dwStateBit, StringBuilder lpszStateBit, int cchStateBitMax);

    /// <summary>
    ///     将指定对象角色文本复制到缓冲区中
    ///     https://learn.microsoft.com/zh-cn/windows/win32/api/oleacc/nf-oleacc-getroletexta
    /// </summary>
    /// <param name="lRole">指定对象</param>
    /// <param name="lpszStateBit">文本缓冲区</param>
    /// <param name="cchStateBitMax">缓冲区最大字符数</param>
    /// <returns>如果成功，并且 lpszRole 为非 NULL，则返回值是 (ANSI 字符串) 或字符数， (Unicode 字符串) 复制到缓冲区（不包括终止 null 字符）的字节数。</returns>
    [DllImport("Oleacc")]
    private static extern int GetRoleText(int lRole, StringBuilder lpszStateBit, int cchStateBitMax);

    /// <summary>
    ///     获取区域
    /// </summary>
    /// <param name="accessible">IAccessible</param>
    /// <returns>Rectangle</returns>
    public static Rectangle GetRectangle(this IAccessible accessible)
    {
        accessible.accLocation(out var x, out var y, out var w, out var h, 0);
        return new Rectangle(x, y, w, h);
    }

    /// <summary>
    ///     获取状态文本
    /// </summary>
    /// <param name="accessible">IAccessible</param>
    /// <returns>状态文本</returns>
    public static string GetStateText(this IAccessible accessible)
    {
        var builder = new StringBuilder();
        int result = GetStateText(accessible.accState[0], builder, 256);
        return builder.ToString();
    }

    /// <summary>
    ///     将指定接口的地址检索到与给定窗口关联的对象
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public static IAccessible? GetAccessibleFromHandle(IntPtr handle)
    {
        IAccessible currentAccessible = null;
        var guid = new Guid(0x618736E0, 0x3C3D, 0x11CF, 0x81, 0xC, 0x0, 0xAA, 0x0, 0x38, 0x9B, 0x71);
        AccessibleObjectFromWindow(handle, -4, ref guid, ref currentAccessible);
        return currentAccessible;
    }


    /// <summary>
    ///     检索对应于IAccess接口的特定实例的窗口句柄
    /// </summary>
    /// <param name="accessible">IAccessible</param>
    /// <returns>窗口句柄</returns>
    public static IntPtr GetWindowFromAccessible(IAccessible accessible)
    {
        WindowFromAccessibleObject(accessible, out var handle);
        return handle;
    }

    /// <summary>
    ///     获取父级对象
    /// </summary>
    /// <param name="target">IAccessible</param>
    /// <returns>父级对象</returns>
    public static IAccessible? GetParent(this IAccessible target)
    {
        return target?.accParent as IAccessible;
    }
}