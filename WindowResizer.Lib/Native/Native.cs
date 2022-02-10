using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace WindowResizer.Lib.Native;

/// docs for pinvoke: https://docs.microsoft.com/en-us/dotnet/standard/native-interop/
/// types: https://docs.microsoft.com/en-us/windows/win32/learnwin32/windows-coding-conventions
internal static class Native
{
    public delegate bool EnumWindowsCallback(IntPtr windowHandle, int lParam);
    
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumwindows
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool EnumWindows(EnumWindowsCallback callback, int lParam);

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowthreadprocessid
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr handle, out uint processId);
    
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowtextlengthw
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowTextLengthW(IntPtr windowHandle);
    
    // /// <summary>
    // /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowtextw
    // /// </summary>
    // [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    // public static extern int GetWindowTextW(IntPtr windowHandle, StringBuilder windowTitleStringBuilder, int nMaxCount);

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowtextw
    /// </summary>
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern int GetWindowTextW(IntPtr windowHandle, char[] lpString, int nMaxCount);

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowrect
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetWindowRect(IntPtr windowHandle, out Rect rect);
    
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public readonly struct Rect
    {
        public readonly int Left;
        public readonly int Top;
        public readonly int Right;
        public readonly int Bottom;

        public int Width => Right - Left;
        public int Height => Bottom - Top;
    }
    
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-iswindowvisible
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool IsWindowVisible(IntPtr windowHandle);
    
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr windowHandle, IntPtr insertAfter, int x, int y, int width, int height, SetWindowPosFlags flags);

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
    /// </summary>
    [Flags]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum SetWindowPosFlags : uint
    {
        /// Retains the current position (ignores X and Y parameters). 
        SWP_NOMOVE = 0x0002
    }
}
