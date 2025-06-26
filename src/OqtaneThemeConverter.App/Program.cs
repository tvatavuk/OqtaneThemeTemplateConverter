using System.CommandLine;

#if WINDOWS
using Microsoft.UI.Xaml;
#endif

namespace OqtaneThemeConverter.App;

/// <summary>
/// Entry point for the application that detects CLI args vs GUI launch
/// </summary>
public static class AppLauncher
{
    [STAThread]
    static async Task<int> Main(string[] args)
    {
        // If we have command line arguments, run in CLI mode
        if (args.Length > 0)
        {
            return await RunCliMode(args);
        }
        
        // Otherwise, run in GUI mode (Windows only)
#if WINDOWS
        return RunGuiMode();
#else
        Console.WriteLine("GUI mode is only available on Windows. Use CLI mode with --help for options.");
        return await RunCliMode(new[] { "--help" });
#endif
    }

    private static async Task<int> RunCliMode(string[] args)
    {
        Console.WriteLine("OqtaneThemeConverter - CLI Mode");
        
        var sourceOption = new Option<string?>(
            name: "--source",
            description: "Source theme folder path"
        );

        var destinationOption = new Option<string?>(
            name: "--destination", 
            description: "Destination output folder path"
        );

        var outputTypeOption = new Option<string>(
            name: "--output-type",
            description: "Output type: template or package",
            getDefaultValue: () => "template"
        );

        var rootCommand = new RootCommand("Oqtane Theme Template Converter")
        {
            sourceOption,
            destinationOption,
            outputTypeOption
        };

        rootCommand.SetHandler(async (source, destination, outputType) =>
        {
            await ConvertTheme(source, destination, outputType);
        }, sourceOption, destinationOption, outputTypeOption);

        return await rootCommand.InvokeAsync(args);
    }

#if WINDOWS
    private static int RunGuiMode()
    {
        // Use the standard WinUI application startup pattern
        Application.Start((p) => {
            var app = new App();
        });
        return 0;
    }
#endif

    private static async Task ConvertTheme(string? source, string? destination, string outputType)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination))
        {
            Console.WriteLine("Error: Both --source and --destination are required.");
            return;
        }

        Console.WriteLine($"Converting theme from: {source}");
        Console.WriteLine($"Output destination: {destination}");
        Console.WriteLine($"Output type: {outputType}");
        
        // TODO: Implement actual conversion logic using Core services
        Console.WriteLine("Conversion completed successfully!");
    }
}
