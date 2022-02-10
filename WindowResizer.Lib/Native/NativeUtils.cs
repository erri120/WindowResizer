using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowResizer.Lib.Native;

public static class NativeUtils
{
    private static void EnsureLastError()
    {
        var lastError = Marshal.GetLastWin32Error();
        if (lastError == 0) return;
        throw new Win32Exception(lastError);
    }
    
    internal static string GetWindowTitle(IntPtr windowHandle)
    {
        if (windowHandle.Equals(IntPtr.Zero))
            throw new ArgumentException("Invalid Handle!", nameof(windowHandle));
        
        var titleLength = Native.GetWindowTextLengthW(windowHandle);
        EnsureLastError();

        var arrayPool = ArrayPool<char>.Shared;
        var rented = arrayPool.Rent(titleLength);
        
        var actualLength = Native.GetWindowTextW(windowHandle, rented, rented.Length);
        EnsureLastError();

        return new string(rented, 0, actualLength);
    }

    public static IEnumerable<NativeWindow> GetWindows()
    {
        var windows = new List<NativeWindow>();
        var processes = new Dictionary<uint, Process>();
        
        Native.EnumWindows((windowHandle, _) =>
        {
            var threadId = Native.GetWindowThreadProcessId(windowHandle, out var processId);
            EnsureLastError();
            
            if (!processes.TryGetValue(processId, out var process))
            {
                process = Process.GetProcessById((int)processId);
                processes.Add(processId, process);
            }

            var window = new NativeWindow(windowHandle, process);
            windows.Add(window);
            
            return true;
        }, 0);

        return windows;
    }

    internal static (int width, int height) GetWindowDimensions(IntPtr windowHandle)
    {
        if (!Native.GetWindowRect(windowHandle, out var rect)) return (0, 0);
        EnsureLastError();

        return (rect.Width, rect.Height);
    }

    internal static bool IsWindowVisible(IntPtr windowHandle)
    {
        if (Native.IsWindowVisible(windowHandle)) return true;
        EnsureLastError();
        return false;
    }

    internal static void ResizeWindow(IntPtr windowHandle, int newWidth, int newHeight)
    {
        if (Native.SetWindowPos(windowHandle, IntPtr.Zero, 0, 0, newWidth, newHeight, Native.SetWindowPosFlags.SWP_NOMOVE)) return;
        EnsureLastError();
    }
}
