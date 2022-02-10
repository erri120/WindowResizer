using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Cli;
using WindowResizer.Lib.Native;

namespace WindowResizer;

public class ResizeCommand : Command<ResizeCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Path to the executable of the process which window you want to resize")]
        [CommandArgument(0, "[path]")]
        public string? ExecutablePath { get; init; }

        [CommandOption("-w|--width")]
        [DefaultValue(-1)]
        public int Width { get; init; }
        
        [CommandOption("-h|--height")]
        [DefaultValue(-1)]
        public int Height { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        if (settings.ExecutablePath is null)
        {
            AnsiConsole.WriteException(new ArgumentException("Invalid Path!", nameof(settings)));
            return -1;
        }

        if (settings.Width == -1 && settings.Height == -1)
        {
            AnsiConsole.WriteException(new ArgumentException("You must set either width, height or both!", nameof(settings)));
            return -1;
        }

        if (!File.Exists(settings.ExecutablePath))
        {
            AnsiConsole.WriteException(new ArgumentException("File does not exist!", nameof(settings)));
            return -1;
        }

        var windows = NativeUtils.GetWindows();
        var groupedByProcess = windows
            .GroupBy(window => window.Process)
            .ToDictionary(x => x.Key, x => x.ToList());

        var processes = groupedByProcess
            .Where(kv =>
            {
                try
                {
                    return kv.Key.MainModule?.FileName?.Equals(settings.ExecutablePath) ?? false;
                }
                catch
                {
                    return false;
                }
            })
            .ToList();

        if (!processes.Any())
        {
            AnsiConsole.MarkupLine("[red]No matching process found![/]");
            return -1;
        }

        var window = processes.First().Value.First();

        var newWidth = settings.Width;
        var newHeight = settings.Height;
        
        if (newWidth == -1)
        {
            newWidth = window.CalculateNewWidth(newHeight);
        } else if (newHeight == -1)
        {
            newHeight = window.CalculateNewHeight(newWidth);
        }

        window.Resize(newWidth, newHeight);
        
        return 0;
    }
}
