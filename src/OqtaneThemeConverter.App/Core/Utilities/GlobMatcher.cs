using System.IO;
using System.Text.RegularExpressions;

namespace OqtaneThemeConverter.App.Core.Utilities;

/// <summary>
/// Utility for matching file and directory paths against glob patterns
/// </summary>
public static class GlobMatcher
{
    /// <summary>
    /// Checks if a path matches any of the given glob patterns
    /// </summary>
    public static bool IsMatch(string path, IEnumerable<string> patterns)
    {
        return patterns.Any(pattern => IsMatch(path, pattern));
    }
    
    /// <summary>
    /// Checks if a path matches a specific glob pattern
    /// </summary>
    public static bool IsMatch(string path, string pattern)
    {
        // Convert glob pattern to regex
        var regexPattern = GlobToRegex(pattern);
        var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
        
        // Normalize path separators
        var normalizedPath = path.Replace('\\', '/');
        
        return regex.IsMatch(normalizedPath);
    }
    
    private static string GlobToRegex(string glob)
    {
        // Escape special regex characters, but preserve glob wildcards
        var pattern = Regex.Escape(glob)
            .Replace("\\*\\*", ".*")  // ** matches any path
            .Replace("\\*", "[^/]*")  // * matches any filename
            .Replace("\\?", "[^/]");  // ? matches single character
        
        // Ensure pattern matches from start to end
        return "^" + pattern + "$";
    }
}