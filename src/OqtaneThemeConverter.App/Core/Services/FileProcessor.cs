using OqtaneThemeConverter.App.Core.Configuration;
using System.IO;

namespace OqtaneThemeConverter.App.Core.Services;

/// <summary>
/// Handles file processing operations for theme conversion
/// </summary>
public class FileProcessor
{
    private readonly TemplateGeneratorConfig _config;
    
    public FileProcessor(TemplateGeneratorConfig config)
    {
        _config = config;
    }
    
    public async Task ProcessThemeAsync(string sourcePath, string destinationPath, string outputType)
    {
        if (!Directory.Exists(sourcePath))
            throw new DirectoryNotFoundException($"Source directory not found: {sourcePath}");
        
        // Create destination directory if it doesn't exist
        Directory.CreateDirectory(destinationPath);
        
        // Copy and process files
        await CopyAndProcessDirectoryAsync(sourcePath, destinationPath);
    }
    
    private async Task CopyAndProcessDirectoryAsync(string sourceDir, string destDir)
    {
        var dirInfo = new DirectoryInfo(sourceDir);
        
        // Create destination directory
        Directory.CreateDirectory(destDir);
        
        // Process files
        foreach (var file in dirInfo.GetFiles())
        {
            if (ShouldExcludeFile(file.FullName))
                continue;
                
            var destFilePath = Path.Combine(destDir, file.Name);
            await ProcessFileAsync(file.FullName, destFilePath);
        }
        
        // Process subdirectories
        foreach (var subDir in dirInfo.GetDirectories())
        {
            if (ShouldExcludeDirectory(subDir.FullName))
                continue;
                
            var destSubDir = Path.Combine(destDir, subDir.Name);
            await CopyAndProcessDirectoryAsync(subDir.FullName, destSubDir);
        }
    }
    
    private async Task ProcessFileAsync(string sourceFile, string destFile)
    {
        // For now, just copy the file
        // TODO: Implement token replacement logic
        File.Copy(sourceFile, destFile, true);
        await Task.CompletedTask;
    }
    
    private bool ShouldExcludeFile(string filePath)
    {
        // TODO: Implement glob pattern matching
        return false;
    }
    
    private bool ShouldExcludeDirectory(string dirPath)
    {
        var dirName = Path.GetFileName(dirPath);
        return dirName == "node_modules" || dirName == ".git" || 
               dirName == "bin" || dirName == "obj";
    }
}