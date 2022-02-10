using System;
using System.Diagnostics;

namespace WindowResizer.Lib.Native;

public class NativeWindow
{
    public readonly IntPtr Handle;
    public readonly Process Process;
    
    private string? _title;
    private int? _width;
    private int? _height;

    internal NativeWindow(IntPtr handle, Process process)
    {
        Handle = handle;
        Process = process;
    }

    public string GetTitle()
    {
        if (_title is not null) return _title;
        
        _title = NativeUtils.GetWindowTitle(Handle);
        return _title;
    }

    private (int width, int height) GetDimensions()
    {
        if (_width.HasValue && _height.HasValue)
            return (_width.Value, _height.Value);

        var (width, height) = NativeUtils.GetWindowDimensions(Handle);
        
        _width = width;
        _height = height;

        return (width, height);
    }
    
    public int GetWidth()
    {
        var (width, _) = GetDimensions();
        return width;
    }

    public int GetHeight()
    {
        var (_, height) = GetDimensions();
        return height;
    }

    public bool IsVisible() => NativeUtils.IsWindowVisible(Handle);

    public int CalculateNewHeight(int newWidth)
    {
        var height = (double)GetHeight();
        var width = (double)GetWidth();
        return (int)((height / width) * newWidth);
    }

    public int CalculateNewWidth(int newHeight)
    {
        var height = (double)GetHeight();
        var width = (double)GetWidth();
        return (int)((width / height) * newHeight);
    }

    public void Resize(int newWidth, int newHeight)
    {
        NativeUtils.ResizeWindow(Handle, newWidth, newHeight);
        _width = newWidth;
        _height = newHeight;
    }
}
