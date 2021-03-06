# WindowResizer

Utility tool for forcibly resizing a window using the Win32 Api.

## Usage

This tool can be used interactively or as a CLI tool.

### Interactive

1) Select a Process
2) Select a Window of the Process if there are multiple Windows
3) Edit either Width, Height or both

The list of processes and windows you see is a filtered list. The Win32 Api we are using returns all windows, including hidden and empty windows. This tool filters out windows that don't meet the following requirements:

- the window must have a title (as returned by [`GetWindowTextW`](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowtextw))
- the width and height of the window must not be 0
- the window must be visible (as returned by [`IsWindowVisible`](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-iswindowvisible))

### CLI

Usage: `WindowResizer [path] -w|--width -h|--height`

Example: `WindowResizer "C:\\Game\\Game.exe" -w 1000`

The CLI requires a path to the executable which window you want to resize. The tool will get all windows, get the process of each window and find the window which process matches the path you specified. The tool uses the file name of the main module of each process to find the window you want to resize. If the process has multiple windows, the first will be selected.

Similar to the interactive mode, you can specify either width, height or both.

## License

See [LICENSE](LICENSE)
