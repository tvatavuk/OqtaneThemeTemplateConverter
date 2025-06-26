# Oqtane Theme Template Converter – Product Specification

## Overview

The **Oqtane Theme Template Converter** is a Windows desktop application designed to transform an existing Oqtane theme source code folder into a reusable **theme template**. This allows developers to take a working Oqtane theme (with specific names and identifiers) and convert it into a generic template with placeholder tokens (e.g. `[Owner]`, `[Theme]`) that can be used to scaffold new themes via Oqtane’s theme generator. The application supports a graphical user interface for ease of use, as well as a command-line interface (CLI) mode for automated or scriptable conversions. The output can be either a folder structure suitable for placement in the Oqtane server’s `wwwroot/Themes/Templates` directory, or a packaged NuGet `.nupkg` file for installing a theme directly into an Oqtane installation. The tool ensures that all occurrences of company/theme-specific identifiers in filenames and file contents are replaced with tokens, while preserving the overall project structure and metadata.

## Functional Features

* **Source Theme Selection:** Users can specify a local source directory containing the Oqtane theme’s source code (e.g. a Visual Studio solution with client and package projects). This folder typically includes the theme’s Razor components, C# code, static assets, resource files, project files, and manifest (such as `theme.json` or `template.json`). The application will read necessary metadata (like theme name, version, owner) from the theme’s manifest file for use in the conversion.
* **Token Mapping Configuration:** The converter uses a JSON configuration file (e.g. `template-generator.config.json`) to determine which strings in the source should be replaced with token placeholders. This config defines a **map of literal strings to token values**. For example, it might map the company name and theme name (such as `"ToSicCre8magic"` and `"Basic"`) to tokens like `"[Owner]"` and `"[Theme]"` respectively. Multiple replacement rules can be defined, each with its own file pattern scope and exclusion patterns. This allows flexible control over which files or file types the replacements apply to.
* **Token Replacement in File **Contents****: The application will scan and process all text-based files in the source directory (e.g. `.cs`, `.razor`, `.csproj`, `.nuspec`, `.json`, `.scss`, etc.) and perform a global find-and-replace of the specified strings with their token placeholders. This ensures that class names, namespaces, GUIDs or IDs containing the theme’s name or owner, and any hard-coded references (such as in resource files or JavaScript) are generalized. For instance, if the source theme’s namespace is `ToSic.Cre8magic.Theme.Basic`, it would be replaced with `[Owner].Theme.[Theme]` throughout the template output. (In Oqtane’s default templates, the **Owner Name** and **Theme Name** provided by the user are inserted into the code namespace, so our converted template must use `[Owner]` and `[Theme]` tokens in those places.)
* **Token Replacement in File and Folder **Names****: In addition to file contents, the tool will also rename files and directories that include the target strings. Any folders or filenames containing the original theme’s identifiers are replaced with the token equivalents. For example, a file named `ToSic.Cre8magic.Theme.Basic.Client.csproj` would be renamed to `[Owner].Theme.[Theme].Client.csproj`, and the folder `ToSic.Cre8magic.Theme.Basic.Client` would become `[Owner].Theme.[Theme].Client`. This ensures the entire project structure of the theme is templatized. Substrings in names are replaced similarly (case-sensitive by default, preserving the original casing of tokens for consistency).
* **Exclusion of Unwanted Files/Folders:** The conversion process will skip over certain files and directories that should not be included in the template. These exclusions are defined via glob patterns in the configuration. By default, common development artifacts and dependencies like version control folders or Node modules are excluded. For example, the tool will ignore directories such as `.git/` and `node_modules/`, any build output folders (e.g. `bin/`, `obj/`), and other irrelevant files that are not part of the theme’s source template. This keeps the generated template lean and free of unnecessary content.
* **Incorporation of Theme Metadata:** The application reads additional metadata from the theme’s manifest (e.g. a `theme.json` or `template.json` file in the source). This may include properties like the theme’s title, version number, author/owner name, package name, and other settings. These values can be used to:

  * Populate the template’s own descriptor (for example, creating or updating a `template.json` in the output with the appropriate Title and Oqtane compatibility version).
  * Ensure that any version numbers or package identifiers in the project files or NuGet specifications are either tokenized or updated appropriately.
  * Log identifying information about the source theme (e.g. “Converting theme ‘Basic’ by ToSic, version 1.0.0”).
* **Output as Template Folder:** The primary output option is a folder structure that represents the theme template. This folder is arranged exactly like an Oqtane theme, but with all specific names replaced by tokens. The user can take this folder and copy it into the Oqtane server under `wwwroot/Themes/Templates/YourTemplateName` to make it available as a template for new themes. Oqtane’s “Create Theme” wizard will then list this template, and when selected, it will prompt for an Owner Name and Theme Name and generate a new theme by replacing the `[Owner]` and `[Theme]` tokens with the values provided. (For example, if a template uses `[Owner]` = "Example" and `[Theme]` = "Template" as placeholders, and the user inputs Owner = "ACME" and Theme = "Rocket", the new theme’s files and code will use `ACME` and `Rocket` accordingly.)
* **Output as NuGet Package (.nupkg):** As an alternative, the application can package the theme into a NuGet `.nupkg` file for direct installation. This is useful if the goal is to distribute the theme itself (rather than a template). In this mode, the tool will **compile** the theme (if source code is provided) or otherwise bundle the prepared files into a NuGet package format. The package will include the compiled theme DLL(s), the `theme.json`/`template.json` manifest, and any static assets, matching the structure Oqtane expects for theme installation. The version number and other NuGet metadata will be derived from the theme’s manifest. The resulting `.nupkg` can be uploaded via Oqtane’s Theme Management interface (under “Upload”) to install the theme. (When using the CLI, a user might opt for this output to automate building and packaging the theme for deployment.)
* **Graphical User Interface (WinUI):** The application provides an intuitive WinUI-based GUI for users who prefer a visual tool. The GUI includes fields to select the source folder and destination, options to choose the output type (template folder or package), and buttons to execute the conversion. It also displays a live log and a progress bar so the user can monitor the conversion steps (described in detail in the *WinUI Layout* section).
* **Command-Line Interface Mode:** For advanced use cases or CI/CD integration, the converter supports running with command-line arguments. When invoked with the appropriate parameters (source path, destination path, etc.), the application will perform the conversion headlessly. In CLI mode it produces minimal console output (e.g. success/failure messages or high-level progress) and writes detailed logs to a file. This allows automation – for instance, a developer could script the conversion of a theme as part of a build process. If the application is launched via CLI with all required arguments, it will **not** open the GUI, and will instead run the conversion and then exit. (Launching the executable without CLI arguments will open the GUI by default.)
* **Logging and Progress Tracking:** All operations performed by the tool (such as file copies, replacements made, files skipped, errors, etc.) are recorded in a log. The log is written to a timestamped log file on disk for review or debugging. In the GUI, a scrollable log viewer pane shows real-time progress of these operations. The log includes each significant action (e.g. “Replaced 12 occurrences of `Basic` with `[Theme]` in file X”, “Copied and renamed file Y to ...”, “Skipped node\_modules (excluded)”). A progress bar or progress indicator is shown to reflect the overall completion status of the conversion, giving feedback especially if the process takes more than a few seconds. Upon completion, the tool may also present a summary (e.g. number of files processed, any warnings or errors). If an error occurs, it is highlighted in the log and the tool will attempt to continue processing remaining files where possible, then report which issues occurred.
* **Minimal CLI Output:** In CLI mode, the tool runs quietly by default, suitable for scripts. It will output only essential information to the console, such as the start of the process, any critical errors, and a final success message with the location of the output. Detailed information (equivalent to the GUI log) will still be written to the log file, but the console won’t be cluttered with verbose messages unless a verbose flag is provided. This design ensures the CLI can be used in automated environments (like build servers) without producing excessive output, while still providing full details in the log file for troubleshooting if needed.

## Technical Requirements

* **Target Platform:** Windows 10 or higher, 64-bit. The application is a Windows Desktop app and leverages the **WinUI 3** framework for its GUI. It will run as a native Windows app with a standard installer or as a standalone executable. The app must function offline (all operations are local file system based).
* **.NET 9.0 Self-Contained Application:** The converter is built on **.NET 9.0** and distributed as a **self-contained** executable. This means users do not need to have any specific .NET runtime installed – the necessary runtime components are bundled with the app. The project will be published with trimming and single-file bundling enabled, to reduce size and simplify deployment. The .csproj publish settings include, for example:

  ```xml
  <PublishTrimmed>true</PublishTrimmed>  
  <TrimMode>link</TrimMode>  
  <PublishSingleFile>true</PublishSingleFile>  
  <SelfContained>true</SelfContained>  
  <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>  
  <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>  
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>  
  <TrimmerDefaultAction>link</TrimmerDefaultAction>
  ```

  These settings ensure the output is a single `.exe` file, compressed and trimmed of unused framework libraries, targeting 64-bit Windows. The use of `<TrimmerDefaultAction>link</TrimmerDefaultAction>` with trimming helps minimize the app’s size by linking only the used assemblies, but care will be taken (via dynamic dependency preservation attributes or config) to avoid trimming essential reflection-used code (such as WinUI components or JSON serialization classes needed at runtime).
* **Performance and Memory:** The application should handle medium-sized theme projects efficiently. An Oqtane theme might consist of dozens of files (C# files, Razor files, JavaScript/TypeScript, CSS/SCSS, images, etc.). The tool will copy and process these files in memory-efficient streams to avoid high memory usage. For instance, it will perform replacements line by line or in buffered chunks rather than reading very large files entirely into memory when not necessary. The progress bar should remain responsive during the operation.
* **File I/O and Permissions:** The tool requires read access to the source directory and write access to the destination directory. It should handle errors gracefully if a file is unreadable or if a destination path is not writable, by logging the error and continuing with remaining files. If run as a normal user, it is assumed the source and destination are in user-accessible locations. (No elevated privileges are needed beyond standard file access.)
* **Error Handling and Validation:** The application will validate input parameters: e.g., check that the source directory exists and contains an Oqtane theme (it might check for expected files like `*.Client.csproj`, `theme.json`/`template.json`, etc.), and that the destination is either empty or can be created. If packaging to `.nupkg`, it will ensure that required build tools or packaging steps can succeed (for example, if using MSBuild, the .NET SDK must be present, and if not, it will alert the user or disable the package option). Any missing config file or misconfigured JSON (for token map) will result in a clear error message.
* **Framework and Dependencies:** The application is built with .NET 9 and WinUI; it uses standard libraries for file handling (System.IO) and JSON (System.Text.Json or similar) to read the config and theme manifest. No additional heavy external dependencies are introduced, to keep the self-contained build small. If NuGet packaging is implemented, it may use the NuGet.Library or .NET CLI under the hood, but preferably without requiring the user to manually install anything. For example, the app could include a NuGet packaging library or use `dotnet pack` via a bundled MSBuild if available.
* **Compatibility:** The produced template folder will be compatible with the Oqtane version corresponding to the source theme. For example, if the source theme targets Oqtane 3.x or 4.x, the template’s metadata (like the `template.json` Version field) will reflect that. The app might allow specifying or automatically detecting the Oqtane framework version (perhaps via the `template.json` or project references in the source). This ensures that when the template is used, the scaffolded theme will target the correct Oqtane framework reference.
* **Security Considerations:** All operations are local. The tool does not execute untrusted code – it only performs text replacement and file operations. However, if packaging into a `.nupkg`, it will run build scripts or commands that are part of the theme’s project (like MSBuild on the `.csproj` or any included build scripts such as `release.cmd`). These will be executed in a controlled manner. The application should sanitize or validate any path inputs to prevent path traversal issues (especially if it ever accepts relative paths or is used in automation).

## WinUI Layout and User Interface Design

The **WinUI GUI** is designed to be user-friendly, grouping all necessary inputs and actions on a single window. Below is the breakdown of the main interface elements and their behavior:

* **Source Directory Picker:** A text field or combo box where the user specifies the path to the source theme folder. This is accompanied by a “Browse…” button that opens a folder picker dialog. The field should display the selected path. Validation: if the path does not exist or does not appear to contain a valid Oqtane theme (for instance, if required files are missing), the app will show an error message (e.g. “Please select a valid Oqtane theme source folder”).
* **Destination Directory Picker:** A text field (with “Browse…” button) for the output path. This can be a non-existent folder (which will be created) or an empty folder. If the folder exists and is not empty, the app will prompt the user for confirmation or require an empty folder to avoid overwriting unintended files. In the GUI, this could also be a save dialog especially if output mode is a `.nupkg` (in that case, the user might select a file path for the package rather than a folder).
* **Configuration File Selection (optional):** By default, the application looks for `template-generator.config.json` in its working directory or alongside the executable. However, the GUI can include an advanced option to specify a custom config file path (in case users have multiple mapping files for different themes). This could be hidden under an “Advanced” expander. If not specified, it will use the default mapping. The GUI should parse and validate the JSON on load (or when conversion starts) and show an error if the JSON is invalid.
* **Output Format Options:** A set of controls to choose the output type. This could be a pair of radio buttons or a dropdown:

  * “**Template Folder**” – when selected, the output will be a folder containing the tokenized theme template (suitable for Oqtane’s `Themes/Templates` directory).
  * “**NuGet Package (.nupkg)**” – when selected, the output will be a compiled NuGet package of the theme. If this is chosen, the UI might enable additional fields (for example, a text box for version number or package name override, if the user wants to specify them; otherwise it will use the theme’s manifest data).
* **Convert/Generate Button:** A prominent button (e.g. “Generate Template” or “Convert Theme”) that initiates the conversion process. This button will be enabled only when the required inputs (source, destination, etc.) are provided and valid. Upon clicking, the app will start the conversion in a background task, allowing the UI to update progress. The button may then be disabled or changed to “Cancel” if mid-process cancellation is supported.
* **Progress Indicator:** A progress bar (horizontal bar) or a progress ring is displayed to give feedback during the conversion. Since the number of steps (files to process) can be determined in advance, a progress **bar** with percentage completion can be used. For example, if there are 100 files to copy/transform, the bar will update as each file is processed. Alongside the bar, a text label might show the current operation (“Processing file X of Y: filename…”). In case of packaging (which may involve running a build), the progress might show steps like “Building project…” and “Creating NuGet package…”.
* **Log Output Area:** A multi-line, scrollable text area is embedded in the window (often below the progress bar or in a tab control) to display the log of operations in real-time. Each significant action or message appends a new line to this text box. For example:

  * “Reading configuration from template-generator.config.json…”
  * “Replacing tokens in file: Client\Themes\Default.razor – replaced 3 occurrences.”
  * “Renamed file: ToSic.Cre8magic.Theme.Basic\Client\ThemeInfo.cs -> \[Owner].Theme.\[Theme]\Client\ThemeInfo.cs”
  * “Excluded folder node\_modules – skipped.”
  * “Packing NuGet package…”
  * “Completed successfully. Template output ready at C:\Output\MyThemeTemplate.”
    The log viewer allows the user to scroll back through messages. It could be read-only and possibly have a context menu to copy text. For clarity, different message types could be prefixed (e.g. “\[INFO]”, “\[ERROR]”) or colored (errors in red).
* **Save Log / View Log File:** Optionally, the UI can provide a button like “Open Log File” or “Save Log” that lets the user open the full log file in an external editor. However, since the log is auto-saved to a file, this is a convenience. The path to the log file can be shown or easily accessible (for example, clicking “Open Log File” would launch it in Notepad).
* **Status Notifications:** The GUI might use a status bar or popup notifications for brief messages. For instance, when conversion finishes, a non-intrusive dialog or a status text could say “Conversion completed” (or “Conversion failed – see log for details” if errors occurred). If the user tries to start conversion with invalid inputs, a message box will explain what needs to be corrected.
* **Layout and Aesthetics:** The overall layout is a single-window form with logical grouping of elements. The top section contains input fields (Source, Destination, Config) and output options. The middle section has the action button and progress bar. The bottom section is the log output. Proper padding and labels are used so the interface is self-explanatory. For example, each text box has a label (“Source Theme Folder:” above it, “Destination Output:” above it). The window is resizable, and the log area expands as needed when the window is resized (important for reading long logs). WinUI styling is applied for a modern look consistent with Windows 10/11 guidelines.

## Command-Line Interface Parameters and Behavior

When run from the command line, the application accepts various parameters to control its operation. The CLI is designed such that if any required parameter is missing, or if `--help` is invoked, it will display a usage message describing these options. **Launching without any arguments will start the GUI**, whereas providing arguments triggers the headless conversion. Key CLI arguments include:

* **`-s, --source <path>`** – **Required.** The file system path to the source Oqtane theme folder to convert. This should point to the root of the theme’s solution or project folder (the same one you would select in the GUI). Example: `-s "C:\Dev\MyTheme.Basic"`
* **`-d, --destination <path>`** – **Required.** The target path where the converted output will be placed. If the path does not exist, the tool will create it. If it exists and `--overwrite` is not set (see below), the tool will refuse to proceed to avoid overwriting files. For a template output, this is a directory; for a package output, this can be a directory (where the `.nupkg` will be saved) or a file path ending in `.nupkg`.
* **`-c, --config <file>`** – *(Optional)* Path to a custom JSON config file defining replacement mappings and rules. If not provided, the tool will look for `template-generator.config.json` in the current directory or the application’s directory. This allows using different token maps for different themes.
* **`-o, --output <type>`** – *(Optional)* Specifies the output type. Accepted values could be `folder` (or `template`) and `package`. For example, `-o folder` means produce a template folder, while `-o package` means produce a NuGet package. If not provided, the default mode is `folder` (template folder output). Alternatively, the presence of a `-p, --package` flag could toggle this; but using `-o` with explicit type is clearer for a productized tool.
* **`--overwrite`** – *(Optional)* If set, the tool will overwrite the destination if it already exists. In template folder mode, this means it may replace files in an existing directory. In package mode, if a file path is given and a file by that name exists, it will overwrite it. By default, without this flag, the tool will error out if the destination exists (to protect from accidental data loss).
* **`-q, --quiet`** – *(Optional)* Quiet mode. When this flag is present, the console output is minimal (no interactive prompts, no informational messages). Only errors or a final success line are printed. This is useful for scripting. In quiet mode, it assumes “yes” for any prompts (for example, it would behave as if `--overwrite` was used for existing destinations, or it might fail immediately if something is amiss rather than asking). If not in quiet mode and the CLI encounters a situation like destination exists or config missing, it may prompt or halt with an explanation.
* **`-v, --verbose`** – *(Optional)* Opposite of quiet, this would force more detailed logging to console. In most cases, the default without `-q` is already quite minimal, deferring to the log file for details. A `--verbose` could echo the log to console or include debug info if needed. This is mainly for troubleshooting.
* **`--help`** – Displays usage information. Lists all the available arguments with descriptions. The tool should handle `-h` or `/?` as aliases for help as well, for user convenience.

**CLI Behavior:** When run with valid arguments, the tool will perform the following steps in sequence:

1. **Validation:** It checks that source exists and is accessible, and that destination is writable or can be created. It also validates the JSON configuration (if provided or default) can be loaded. If any check fails, it will output an error to console and exit with a non-zero status code (for integration into scripts/CI, where that can signal failure). If all checks pass, it proceeds.
2. **Conversion Process:** It performs the conversion just as in GUI mode – copying files, replacing tokens, renaming items, etc. The difference is, instead of updating a GUI, it will print simple progress indicators to the console. For example, it might print “Processing 25 files…” at start, then perhaps a dot “.” or a file count every few files to show it’s active, or it might just silently work and then print “Done.” at the end (depending on quiet/verbose settings). Errors encountered will be printed to console (unless quiet mode, where they might just abort). If not quiet, it could also list any warnings at the end (e.g. “5 files were skipped due to errors – see log.txt for details”).
3. **Logging:** All detailed logs of operations will be written to a log file even in CLI mode. The log file path could be a fixed location (like `TemplateConverter.log` in the output or current directory, possibly with a timestamp) or specified via an argument. The console will output the path of the log file at the end for reference (e.g. “See TemplateConverter\_20250626.log for full details.”).
4. **Exit Codes:** The application will return a success exit code (0) if conversion completes without critical errors. If errors occurred that likely resulted in an incomplete conversion, it will return a non-zero exit code. This way, automated scripts can detect failure.

**Example Usage:**

```
OqtaneThemeConverter.exe --source "C:\Dev\ToSic.Cre8magic.Theme.Basic" \
    --destination "C:\Dev\Templates\Cre8magicBasicTemplate" --output folder
```

This would take the **Cre8magic Basic** theme source and output a tokenized template folder to the specified destination. After running, the console might show:

```
Reading config... Replacing tokens... Completed successfully.
```

And the resulting template folder can be copied into `wwwroot/Themes/Templates` on an Oqtane server to be available for new theme creation. On the other hand, using `--output package` might be:

```
OqtaneThemeConverter.exe -s "C:\Dev\ToSic.Cre8magic.Theme.Basic" -d "C:\Dev\Packages" -o package
```

This would produce a file like `ToSic.Cre8magic.Theme.Basic.1.0.0.nupkg` (using the theme’s package name and version from its manifest) in the `C:\Dev\Packages` directory, which can then be uploaded to an Oqtane site to install the theme.

## Token Replacement Logic

One of the core functions of the application is the **token replacement engine**, which generalizes a concrete theme into a template by substituting specific identifiers with placeholders. This is governed by the configuration file (`template-generator.config.json`) and the following logic:

* **Configuration Format:** The JSON config contains an array of replacement rule sets, under a property like `"replacements"`. Each rule set can specify:

  * **`files`**: an array of file pattern globs (e.g. `["**/*.*"]` to match all files, or `["**/*.cs", "**/*.razor"]` to limit to certain extensions). This defines which files the rule applies to.
  * **`exclude`**: an array of exclude patterns (optional). Any file or folder matching these globs will be skipped entirely for this rule. Common exclusions here would be `**/node_modules/**`, `**/.git/**`, or perhaps certain binary file types like `**/*.png` if binary content should be left untouched.
  * **`map`**: an object defining the string replacements to perform. Each key is a literal string to find in files/names, and its value is the token (or replacement string) to substitute. For example:

    ```json
    {
      "map": {
         "ToSicCre8magic": "[Owner]",
         "Basic": "[Theme]"
      }
    }
    ```

    This mapping indicates that wherever the sequence “ToSicCre8magic” appears, it should be replaced with “\[Owner]”, and “Basic” replaced with “\[Theme]”. The values in brackets are the placeholders that will remain in the template. *(Note: These tokens are chosen to align with Oqtane’s templating conventions; when a new theme is created from this template, Oqtane will prompt for Owner and Theme values to substitute these.)* Multiple mappings can be present; the replacement engine will apply all of them. It’s recommended that the map keys are ordered from more specific/longer strings to shorter to avoid unintended partial replacements (the implementation can ensure a stable replace order).
* **Case Sensitivity:** By default, replacements are case-sensitive (since identifiers in code are case-sensitive). “Basic” will not match “basic” in content, for example. This prevents accidentally changing words in documentation or code comments that aren’t meant to be tokens. If needed, the config could allow a flag for case-insensitive matching on certain entries, but generally the mapping should explicitly list all variants if necessary.
* **Scanning and Replacing in File Contents:** For each file that matches the inclusion patterns and not the exclusions, the tool reads the file as text (assuming UTF-8 or the system default encoding – the tool will handle common text encodings). It then performs the replacements for every mapping key found. This can be done by reading the entire text into memory or streaming through it. Given typical file sizes (code files), reading fully and using a string replace for each mapping is straightforward and allows multiple passes. The implementation ensures that replacements do not overlap or repeat (for example, if one token’s replacement contains another token string, it should handle that carefully or design mappings to avoid such scenarios). After replacements, the modified text is written to the corresponding file in the destination directory (using the same relative path structure as source, which may be adjusted if directory names changed – see next point).
* **Renaming Files and Directories:** The tool also must apply the token mapping to file and folder names. This is done when creating the file structure in the destination: as it iterates through the source directories, it will create corresponding directories in the output, but use replaced names. For example, a source folder `ToSic.Cre8magic.Theme.Basic.Client` becomes `[Owner].Theme.[Theme].Client` in the output. Similarly, a file `Theme.Basic.sln` (if it existed) might become `Theme.[Theme].sln` (though in practice the solution name likely includes the owner too). The rename logic needs to ensure that the full original token (key) is present in the name to avoid partial renames on unintended substrings. Typically, this is straightforward (e.g. “Basic” is a distinct token in file names). The tool should construct the new path and ensure no conflicts: e.g., if two different source files only differ by the token string, after replacement they might collide. (In well-structured themes this shouldn’t happen, but the tool could detect if it’s about to overwrite an existing output file and issue a warning or modify the name slightly).
* **Directory Traversal Order:** To safely handle renaming, the application will likely traverse the source file system tree and reproduce it in the destination. A depth-first traversal works, but one must be careful: if a parent folder’s name changes due to token replacement, it should create the new folder name in the output and then proceed inside it. The tool will maintain a map of source paths to destination paths. It’s important that when writing a file’s content, it writes into the already token-renamed directory structure.
* **Exclusions Application:** As the tool walks through files, any path that matches an exclusion pattern is skipped entirely. For example, if `node_modules` is in exclusions, the moment the traversal enters the `Client/src` directory and sees a `node_modules` folder, it will skip descending into it and not copy or process any of its files. Similarly, hidden/system files or git metadata like `.gitignore` could be excluded (or included, depending on whether such files are useful in a template – `.gitignore` might actually be useful to include so the new theme project has it). The config can be adjusted; by default `.git` folder itself would be excluded.
* **Processing Binary or Non-Text Files:** The tool will identify files that should be copied without text processing. Typically, images (`.png`, `.jpg`), compiled libraries (`.dll`) if any, and possibly pre-minified JS/CSS could be considered binary for our purposes. The config’s include patterns can be set to avoid these (e.g. using `*.*` will include all, but one could exclude `**/*.png` etc. if needed). If a binary file (or any file) is included but cannot be safely read as text, the tool should catch the decoding error and decide: it could copy the file as-is (no replacement) to destination, or simply skip it if not critical. For images and such, token replacement is not needed, so copying them is fine. However, if the image’s name contains a token string (say the theme name), the file should be renamed accordingly even if its contents aren’t touched. The tool should handle that by using the same renaming function on all file names, regardless of type, even if content replace is bypassed for non-text files.
* **Use of Theme Manifest Data:** If the theme’s manifest (`theme.json` or `template.json`) contains known keys that correspond to the strings we’re tokenizing (like an “owner”: "ToSic" or “name”: "Basic”), the application can cross-verify that with the config mappings. It might even dynamically construct the mapping if not provided – for example, read “owner” and “name” and suggest mapping those values to `[Owner]` and `[Theme]` if the config doesn’t explicitly list them. In our scenario, we assume the config is pre-defined, but this feature could make the tool more robust (so it’s not hard-coded to particular strings). Regardless, the manifest file itself should be processed: e.g., if the source has `template.json` with `"Title": "Cre8magic Basic"`, the output template’s `template.json` might become `"Title": "[Owner] [Theme]"` or something generalized. The application should replace any occurrence of the specific theme name or owner in that metadata file as well. If the manifest is `theme.json` with fields, then `"owner": "ToSic"` becomes `"[Owner]"` and `"name": "Basic"` becomes `"[Theme]"` in the template version. Likewise, `"PackageName": "ToSic.Cre8magic.Oqtane.Theme.Basic"` would be replaced with something like `"[Owner].Oqtane.Theme.[Theme]"`. These replacements ensure that when the template is used, Oqtane will insert the actual chosen values into those fields for the new theme.
* **Verifying Replacement Success:** After processing, the tool could optionally verify that key strings no longer appear in the output (to catch any missed replacements). For example, it might scan the output for the original theme name or owner string and warn if any instance is still found, indicating a possible oversight in the config or a file that wasn’t processed. This gives confidence that the template is clean of any old references.
* **Example – Default Replacements:** In the provided configuration example, the map is: `"ToSicCre8magic" -> "[Owner]"` and `"Basic" -> "[Theme]"`. This means the combined string “ToSicCre8magic” (which seems to be a concatenation of company and product name) is replaced with the token \[Owner], and the word “Basic” (the theme name) is replaced with \[Theme]. So a class named `ToSic.Cre8magic.Theme.Basic.ThemeSettings` in code would turn into `[Owner].Theme.[Theme].ThemeSettings` in the template. Filenames like `ToSic.Cre8magic.Theme.Basic.nuspec` would become `[Owner].Theme.[Theme].nuspec`. Even asset paths will reflect this: e.g., the theme’s static content directory `wwwroot/Themes/ToSic.Cre8magic.Theme.Basic/` will be tokenized to `wwwroot/Themes/[Owner].Theme.[Theme]/` in the template (this is important because Oqtane by convention looks for theme static assets under a folder named `{Namespace}` under wwwroot, which in templates is tokenized).

By following the above logic, the tool ensures a comprehensive replacement of all theme-specific identifiers, resulting in a clean template that Oqtane can use to generate new themes with any owner/name without leftover references to the original.

## Example Folder Structures (Input vs Output)

To illustrate the transformation, below is an example of a source theme’s structure and the corresponding output template structure with tokens. The example is based on the **"Cre8magic Basic"** theme by ToSic (which has *Owner* “ToSicCre8magic” and *Theme Name* “Basic”). After conversion, all instances of “ToSicCre8magic” and “Basic” are replaced by `[Owner]` and `[Theme]` respectively.

### Source Theme Structure (Simplified)

```
ToSic.Cre8magic.Theme.Basic/               <-- Root folder (source theme)
├── ToSic.Cre8magic.Theme.Basic.sln        <-- Solution file
├── Client/                                <-- Project for theme components
│   ├── ToSic.Cre8magic.Theme.Basic.Client.csproj
│   ├── Themes/
│   │   ├── Default.razor
│   │   ├── Fullscreen.razor
│   │   ├── ThemeSettings.razor
│   │   └── ThemeConstants.cs
│   ├── Containers/
│   │   ├── Container-Main.razor
│   │   └── ContainerSettings.razor
│   ├── Resources/
│   │   └── ToSic.Cre8magic.Theme.Basic/   <-- Resx localization files
│   │       ├── Theme.resx
│   │       ├── ThemeSettings.resx
│   │       └── (etc.)
│   ├── wwwroot/
│   │   └── Themes/
│   │       └── ToSic.Cre8magic.Theme.Basic/   <-- Static assets for the theme
│   │           ├── Theme.css
│   │           └── (other CSS/JS files)
│   ├── ThemeInfo.cs
│   └── _Imports.razor
├── Package/                               <-- Project for packaging (NuGet)
│   ├── ToSic.Cre8magic.Theme.Basic.Package.csproj
│   ├── ToSic.Cre8magic.Theme.Basic.nuspec
│   ├── icon.png
│   └── release.cmd / release.sh (build scripts)
├── template.json                          <-- Template metadata (Oqtane template manifest)
└── theme.json                             <-- Theme metadata (perhaps includes name, version, owner)
```

*(Note: This is a representative structure; actual projects may differ, but the key point is the presence of the theme name “Basic” and the owner string in many places.)*

### Output Template Structure (After Conversion)

```
[Owner].Theme.[Theme]/                        <-- Root folder for template
├── [Owner].Theme.[Theme].sln                 <-- Solution with tokens
├── Client/
│   ├── [Owner].Theme.[Theme].Client.csproj
│   ├── Themes/
│   │   ├── Default.razor
│   │   ├── Fullscreen.razor
│   │   ├── ThemeSettings.razor
│   │   └── ThemeConstants.cs
│   ├── Containers/
│   │   ├── Container-Main.razor
│   │   └── ContainerSettings.razor
│   ├── Resources/
│   │   └── [Owner].Theme.[Theme]/
│   │       ├── Theme.resx
│   │       ├── ThemeSettings.resx
│   │       └── ...
│   ├── wwwroot/
│   │   └── Themes/
│   │       └── [Owner].Theme.[Theme]/
│   │           ├── Theme.css
│   │           └── ...
│   ├── ThemeInfo.cs
│   └── _Imports.razor
├── Package/
│   ├── [Owner].Theme.[Theme].Package.csproj
│   ├── [Owner].Theme.[Theme].nuspec
│   ├── icon.png
│   └── release.cmd / release.sh
├── template.json                             <-- Template manifest (updated Title, Namespace with tokens)
└── theme.json                                <-- Theme metadata (owner/name fields replaced with tokens)
```

In the above output, every occurrence of the original “ToSicCre8magic” and “Basic” has been replaced by `[Owner]` and `[Theme]`. For example:

* The namespace in code files (`namespace ToSic.Cre8magic.Theme.Basic`) would appear as `namespace [Owner].Theme.[Theme]` in the template files.
* File paths for static assets and resource files now contain the tokenized folder name `[Owner].Theme.[Theme]` instead of the actual owner/theme.
* The NuGet specification (`*.nuspec`) and manifest files have placeholders for package ID and names, so that if this template is used to create a new theme, those will be replaced with the new theme’s actual package name (composed from the new owner and theme name).
* The `template.json` might now read for example:

  ```json
  {
    "Title": "[Owner] [Theme]",
    "Version": "1.0.0",
    "Namespace": "[Owner].Theme.[Theme]",
    "Type": "External"
  }
  ```

  indicating it’s a generic template titled by the placeholders. Similarly, `theme.json` (if used) would have `"owner": "[Owner]"` and `"name": "[Theme]"`.

This structure can be zipped or placed directly into the Oqtane installation. If placed in `wwwroot/Themes/Templates/[Owner].Theme.[Theme]/`, Oqtane will recognize “\[Owner] \[Theme]” as a new template option when creating themes. The placeholders will be swapped out for real values provided by the user in the Oqtane UI, resulting in a copy of this structure with actual names. (For instance, choosing this template with Owner “MyCompany” and Theme “NewLook” would generate a folder `MyCompany.Theme.NewLook` with all files named and content adjusted accordingly).

If the output mode was NuGet package, the equivalent compiled assembly and files from the `[Owner].Theme.[Theme]` projects would be bundled. The `.nupkg` would contain a `lib` folder with the compiled DLL, the `.Views.dll` (if any), the `wwwroot` assets, and the `template.json/theme.json` manifest, allowing Oqtane to install the theme directly. The package ID would be something like `[Owner].Oqtane.Theme.[Theme]` (following the original package naming convention) and version as specified.

---

By providing both a comprehensive GUI and a robust CLI, and by adhering to Oqtane’s template format conventions, the **Oqtane Theme Template Converter** makes it straightforward to create shareable theme templates. This enables theme developers to rapidly onboard others with a starting point for new themes, and also facilitates maintaining consistency when multiple themes are derived from a common base. The technical design ensures the output is immediately usable in Oqtane – either for scaffolding new theme projects or for direct theme deployment – streamlining the workflow of Oqtane theme development and distribution.


### Solution Structure

OqtaneThemeConverter/
├── OqtaneThemeConverter.sln
├── src/
│   ├── OqtaneThemeConverter.App/          <-- Single project for both GUI (WPF or WinUI) and CLI
│   │   ├── App.xaml (for WPF) or App.cs (for WinUI)
│   │   ├── MainWindow.xaml (WPF) or MainWindow.xaml (WinUI)
│   │   ├── MainWindow.xaml.cs
│   │   ├── Program.cs                    <-- Entry point detects CLI args vs GUI launch
│   │   └── OqtaneThemeConverter.App.csproj  <-- Includes Publish settings for self-contained single EXE
│   └── OqtaneThemeConverter.Core/
│       ├── Configuration/
│       │   └── TemplateGeneratorConfig.cs
│       ├── Services/
│       │   ├── FileProcessor.cs
│       │   └── TokenReplacer.cs
│       ├── Models/
│       │   └── ThemeMetadata.cs
│       ├── Utilities/
│       │   └── GlobMatcher.cs
│       └── OqtaneThemeConverter.Core.csproj
└── tests/
    └── OqtaneThemeConverter.Tests/
        ├── FileProcessorTests.cs
        ├── TokenReplacerTests.cs
        └── OqtaneThemeConverter.Tests.csproj

### Project Details

#### OqtaneThemeConverter.Core

Contains shared logic used by both CLI and GUI applications.

Implements:
- Configuration loading from JSON.
- Token replacement logic.
- File processing (copy, rename, replace).

#### OqtaneThemeConverter.App (WinUI Desktop Application)

- GUI using WinUI 3 for user-friendly interaction.
- Fields for Source, Destination, Output Type selection.
- Log viewer, progress bar, action buttons.

#### OqtaneThemeConverter.CLI (Command Line Interface)

- Minimal console application using arguments for automation.
- Outputs minimal logging to console.

#### OqtaneThemeConverter.Tests

- Unit tests for Core functionalities.
- Uses xUnit framework.