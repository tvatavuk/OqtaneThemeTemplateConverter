#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace OqtaneThemeConverter.App;

/// <summary>
/// Main window for the WinUI application
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.Title = "Oqtane Theme Template Converter";
        
        // Enable convert button when both paths are set
        SourcePathTextBox.TextChanged += OnPathTextChanged;
        DestinationPathTextBox.TextChanged += OnPathTextChanged;
        
        LogMessage("Application ready.");
    }

    private void OnPathTextChanged(object sender, TextChangedEventArgs e)
    {
        ConvertButton.IsEnabled = !string.IsNullOrWhiteSpace(SourcePathTextBox.Text) && 
                                  !string.IsNullOrWhiteSpace(DestinationPathTextBox.Text);
    }

    private async void BrowseSourceButton_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FolderPicker();
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        picker.FileTypeFilter.Add("*");
        
        // Get the current window's HWND
        var hwnd = WindowNative.GetWindowHandle(this);
        InitializeWithWindow.Initialize(picker, hwnd);
        
        var folder = await picker.PickSingleFolderAsync();
        if (folder != null)
        {
            SourcePathTextBox.Text = folder.Path;
            LogMessage($"Selected source folder: {folder.Path}");
        }
    }

    private async void BrowseDestinationButton_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FolderPicker();
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        picker.FileTypeFilter.Add("*");
        
        // Get the current window's HWND
        var hwnd = WindowNative.GetWindowHandle(this);
        InitializeWithWindow.Initialize(picker, hwnd);
        
        var folder = await picker.PickSingleFolderAsync();
        if (folder != null)
        {
            DestinationPathTextBox.Text = folder.Path;
            LogMessage($"Selected destination folder: {folder.Path}");
        }
    }

    private async void ConvertButton_Click(object sender, RoutedEventArgs e)
    {
        ConvertButton.IsEnabled = false;
        ConversionProgressBar.Visibility = Visibility.Visible;
        StatusTextBlock.Text = "Converting...";
        
        try
        {
            LogMessage("Starting conversion...");
            
            var sourcePath = SourcePathTextBox.Text;
            var destinationPath = DestinationPathTextBox.Text;
            var outputType = OutputTypeComboBox.SelectedIndex == 0 ? "template" : "package";
            
            LogMessage($"Source: {sourcePath}");
            LogMessage($"Destination: {destinationPath}");
            LogMessage($"Output Type: {outputType}");
            
            // Validate paths
            if (!Directory.Exists(sourcePath))
            {
                LogMessage("ERROR: Source folder does not exist.");
                StatusTextBlock.Text = "Error: Source folder not found";
                return;
            }
            
            // TODO: Implement actual conversion logic using Core services
            await Task.Delay(2000); // Simulate work
            
            LogMessage("Conversion completed successfully!");
            StatusTextBlock.Text = "Conversion completed";
        }
        catch (Exception ex)
        {
            LogMessage($"ERROR: {ex.Message}");
            StatusTextBlock.Text = "Error occurred";
        }
        finally
        {
            ConvertButton.IsEnabled = true;
            ConversionProgressBar.Visibility = Visibility.Collapsed;
        }
    }

    private void LogMessage(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        LogTextBlock.Text += $"\n[{timestamp}] {message}";
    }
}
#endif