using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

using CsWin32 = Windows.Win32;

namespace Tenon.Infra.Windows.Win32.Hooks;
public static class KeyboardHook
{
    private static CsWin32.UnhookWindowsHookExSafeHandle _hookExSafeHandle = new CsWin32.UnhookWindowsHookExSafeHandle(IntPtr.Zero);

    public static bool Install()
    {
        _hookExSafeHandle = SetHook(HookCallback);
        return !_hookExSafeHandle.IsInvalid;
    }

    public static void Uninstall()
    {
        if (!_hookExSafeHandle.IsInvalid)
            return;
        CsWin32.PInvoke.UnhookWindowsHookEx(new HHOOK(_hookExSafeHandle.DangerousGetHandle()));
    }

    private static LRESULT HookCallback(int code, WPARAM wParam, LPARAM lParam)
    {
        if (code < 0) return CsWin32.PInvoke.CallNextHookEx(_hookExSafeHandle, code, wParam, lParam);
        if (wParam == CsWin32.PInvoke.WM_KEYDOWN || wParam == CsWin32.PInvoke.WM_SYSKEYDOWN)
        {
            var keyCode = Marshal.ReadInt32(lParam);
            var key = Enum.IsDefined(typeof(Keys), keyCode) ? (Keys)keyCode : Keys.NoName;
            KeyDown?.Invoke(null, new KeyEventArgs(key));
        }

        if (lParam == CsWin32.PInvoke.WM_KEYUP || lParam == CsWin32.PInvoke.WM_SYSKEYUP)
        {
            var keyCode = Marshal.ReadInt32(lParam);
            var key = Enum.IsDefined(typeof(Keys), keyCode) ? (Keys)keyCode : Keys.NoName;
            KeyUp?.Invoke(null, new KeyEventArgs(key));
        }

        return CsWin32.PInvoke.CallNextHookEx(_hookExSafeHandle, code, wParam, lParam);
    }

    private static CsWin32.UnhookWindowsHookExSafeHandle SetHook(HOOKPROC proc)
    {
        using (var module = Process.GetCurrentProcess().MainModule)
        {
            var hookExSafeHandle = CsWin32.PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, proc,
                CsWin32.PInvoke.GetModuleHandle(module.ModuleName), 0);
            return hookExSafeHandle;
        }
    }

    public static event KeyEventHandler KeyDown;
    public static event KeyEventHandler KeyUp;
}