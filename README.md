# Oqtane Theme Template Converter

A .NET 8.0 application that converts existing Oqtane themes into reusable templates with token placeholders.

## Features

- **WinUI Desktop Application**: User-friendly GUI for theme conversion
- **Command Line Interface**: CLI support for automation and scripting
- **Token Replacement**: Converts specific theme names to generic tokens like `[Owner]` and `[Theme]`
- **Flexible Configuration**: JSON-based configuration for replacement rules

## Usage

### GUI Mode
Run the application without arguments to open the WinUI interface:
```
OqtaneThemeConverter.App.exe
```

### CLI Mode
Run with command line arguments for automated conversion:
```
OqtaneThemeConverter.App.exe --source "C:\MyTheme" --destination "C:\Output" --output-type template
```

#### CLI Parameters
- `--source`: Path to the source theme folder
- `--destination`: Path to the output destination
- `--output-type`: Output type (`template` or `package`)

## Configuration

The application uses `template-generator.config.json` to define token replacement rules. Example:

```json
{
  "replacements": [
    {
      "files": ["**/*.cs", "**/*.razor"],
      "replacements": {
        "MyCompany": "[Owner]",
        "MyTheme": "[Theme]"
      }
    }
  ]
}
```

## Requirements

- Windows 10 or higher
- .NET 8.0 runtime (self-contained executable includes runtime)

## Building

```bash
dotnet build
dotnet run
```