using System;
using System.Diagnostics;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Cli;
using WindowResizer.Lib.Native;

namespace WindowResizer;

public static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            if (!args.Any())
            {
                RunInteractive();
            }
            else
            {
                var app = new CommandApp<ResizeCommand>();
                app.Run(args);
            }
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
        }
    }

    private static void RunInteractive()
    {
        AnsiConsole.Write(new Rule("WindowResizer"));
        
        var windows = NativeUtils.GetWindows();
        
        var validWindows = windows.Where(window =>
            !string.IsNullOrWhiteSpace(window.GetTitle()) &&
            window.GetHeight() != 0 &&
            window.GetWidth() != 0 &&
            window.IsVisible());

        var groupedByProcess = validWindows
            .GroupBy(window => window.Process)
            .ToDictionary(x => x.Key, x => x.ToList());

        // TODO: look into Choice Groups
        
        var process = AnsiConsole.Prompt(
            new SelectionPrompt<Process>()
                .Title("Select a Process")
                .PageSize(10)
                .AddChoices(groupedByProcess.Keys.OrderBy(x => x.ProcessName))
                .UseConverter(p => $"{p.ProcessName} [grey]({p.MainModule?.FileName})[/]"));

        var processWindows = groupedByProcess[process];

        var selectedWindow = processWindows.Count == 1
            ? processWindows.First()
            : AnsiConsole.Prompt(
                new SelectionPrompt<NativeWindow>()
                    .Title("Select a Window")
                    .PageSize(10)
                    .AddChoices(processWindows)
                    .UseConverter(w => $"{w.GetTitle()}\": {w.GetWidth()}x{w.GetHeight()}"));

        AnsiConsole.Write(new Rule($"{selectedWindow.GetTitle()}"));
        AnsiConsole.WriteLine($"Current Dimensions: {selectedWindow.GetWidth()}x{selectedWindow.GetHeight()}");

        var mode = AnsiConsole.Prompt(
            new SelectionPrompt<EditMode>()
                .Title("Select Edit Mode")
                .AddChoices(EditMode.EditWidthAndHeight, EditMode.EditWidth, EditMode.EditHeight)
                .UseConverter(i =>
                {
                    return i switch
                    {
                        EditMode.EditWidthAndHeight => "Edit [green]Width[/] and [green]Height[/]",
                        EditMode.EditWidth => "Edit [green]Width[/] and calculate Height",
                        EditMode.EditHeight => "Edit [green]Height[/] and calculate Width",
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }));

        var widthPrompt = new TextPrompt<int>("New Width:");
        var heightPrompt = new TextPrompt<int>("New Height:");

        int newWidth;
        int newHeight;
        
        switch (mode)
        {
            case EditMode.EditWidthAndHeight:
            {
                newWidth = AnsiConsole.Prompt(widthPrompt);
                newHeight = AnsiConsole.Prompt(heightPrompt);
                break;
            }
            case EditMode.EditWidth:
            {
                newWidth = AnsiConsole.Prompt(widthPrompt);
                newHeight = selectedWindow.CalculateNewHeight(newWidth);
                
                AnsiConsole.WriteLine($"New Height: {newHeight}");
                break;
            }
            case EditMode.EditHeight:
            {
                newHeight = AnsiConsole.Prompt(heightPrompt);
                newWidth = selectedWindow.CalculateNewWidth(newHeight);
                
                AnsiConsole.WriteLine($"New Width: {newWidth}");
                break;
            }
            default: throw new ArgumentOutOfRangeException();
        }
        
        selectedWindow.Resize(newWidth, newHeight);
    }
}
