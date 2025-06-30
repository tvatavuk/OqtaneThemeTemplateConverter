using OqtaneThemeConverter.App.Core.Configuration;
using System.Text.RegularExpressions;

namespace OqtaneThemeConverter.App.Core.Services;

/// <summary>
/// Handles token replacement in file contents and names
/// </summary>
public class TokenReplacer
{
    private readonly TemplateGeneratorConfig _config;
    
    public TokenReplacer(TemplateGeneratorConfig config)
    {
        _config = config;
    }
    
    public async Task<string> ProcessFileContentAsync(string filePath, string content)
    {
        var processedContent = content;
        
        foreach (var rule in _config.Replacements)
        {
            foreach (var replacement in rule.Replacements)
            {
                processedContent = processedContent.Replace(replacement.Key, replacement.Value);
            }
        }
        
        return await Task.FromResult(processedContent);
    }
    
    public string ProcessFileName(string fileName)
    {
        var processedName = fileName;
        
        foreach (var rule in _config.Replacements)
        {
            foreach (var replacement in rule.Replacements)
            {
                processedName = processedName.Replace(replacement.Key, replacement.Value);
            }
        }
        
        return processedName;
    }
    
    public string ProcessDirectoryName(string directoryName)
    {
        return ProcessFileName(directoryName);
    }
}