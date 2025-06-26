using System.CommandLine;

#if WINDOWS
using Microsoft.UI.Xaml;
#endif

namespace OqtaneThemeConverter.App;

#if WINDOWS
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private Window? _mainWindow;

    /// <summary>
    /// Initializes the singleton application object.
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _mainWindow = new MainWindow();
        _mainWindow.Activate();
    }
}
#endif

/// <summary>
/// Entry point for the application that detects CLI args vs GUI launch
/// </summary>
public static class Program
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
        // Initialize WinUI app
        WinRT.ComWrappersSupport.InitializeComWrappers();
        Application.Start((p) => {
            var context = new DispatcherQueueSynchronizationContext(
                DispatcherQueue.GetForCurrentThread()
            );
            SynchronizationContext.SetSynchronizationContext(context);
            new App();
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
