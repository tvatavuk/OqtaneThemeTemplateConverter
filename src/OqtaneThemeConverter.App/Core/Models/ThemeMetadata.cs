using System.Text.Json.Serialization;

namespace OqtaneThemeConverter.App.Core.Models;

/// <summary>
/// Represents theme metadata from theme.json or template.json
/// </summary>
public class ThemeMetadata
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";
    
    [JsonPropertyName("owner")]
    public string Owner { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;
    
    [JsonPropertyName("oqtaneVersion")]
    public string OqtaneVersion { get; set; } = "5.0.0";
}