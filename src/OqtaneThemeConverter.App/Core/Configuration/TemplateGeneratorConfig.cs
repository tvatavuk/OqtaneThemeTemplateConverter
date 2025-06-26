using System.Text.Json.Serialization;

namespace OqtaneThemeConverter.App.Core.Configuration;

/// <summary>
/// Configuration for the template generator
/// </summary>
public class TemplateGeneratorConfig
{
    [JsonPropertyName("replacements")]
    public List<ReplacementRule> Replacements { get; set; } = new();
    
    [JsonPropertyName("excludePatterns")]
    public List<string> ExcludePatterns { get; set; } = new()
    {
        "**/node_modules/**",
        "**/.git/**",
        "**/bin/**",
        "**/obj/**",
        "**/*.png",
        "**/*.jpg",
        "**/*.jpeg",
        "**/*.gif",
        "**/*.ico"
    };
}

/// <summary>
/// Represents a replacement rule for token substitution
/// </summary>
public class ReplacementRule
{
    [JsonPropertyName("files")]
    public List<string> Files { get; set; } = new() { "**/*.*" };
    
    [JsonPropertyName("exclude")]
    public List<string> Exclude { get; set; } = new();
    
    [JsonPropertyName("replacements")]
    public Dictionary<string, string> Replacements { get; set; } = new();
}