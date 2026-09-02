# timActionCenter

timActionCenter is a configurable, command-driven action panel for Windows. It runs in the system tray and displays a canvas of tiles that can launch applications, execute scripts, or represent on/off actions.

![timActionCenter](https://i.imgur.com/Idvlvep.png)

The application ships with Proxy, Theme, and Hibernate tiles as examples. These features are defined in JSON like every other tile; the application itself only understands generic action and toggle behavior.

## Requirements

- Windows 10 or Windows 11
- PowerShell is required by the three default tile definitions
- .NET 10 Desktop Runtime when building or running a framework-dependent build from source

## Installation

Download the latest package from the [Releases page](https://github.com/thisismalindu/timActionCenter/releases), extract it, and run `timActionCenter.exe`.

The application starts in the system tray instead of opening a normal taskbar window.

Release ZIPs produced by this repository are self-contained and do not require a separate .NET installation.

Extract the application to its permanent folder before enabling **Start with Windows**, because the startup entry points to the current executable location.

## Using the application

### Opening the tile canvas

Click the timActionCenter tray icon to open the tile canvas. The window hides when it loses focus.

Clicking a tile executes the commands stored in that tile's configuration. Toggle tiles use the accent style when their state command reports that they are active.

### Adding an action tile

Click the pencil button in the lower-right corner and select `Action` as the behavior.

Complete these fields:

- **Title:** Text displayed below the icon.
- **Description:** Tooltip and accessibility description.
- **Icon:** A glyph loaded directly from the bundled Segoe Fluent Icons font.
- **Command or file path:** An executable, script host, document, URL, or other shell-openable target.
- **Arguments:** Optional arguments passed to the command.
- **Working directory:** Optional starting directory for the command.
- **Wait for the command to finish:** Keeps the tile disabled until the process exits.
- **Run without showing a window:** Starts console commands without displaying a terminal window.

For example, use `notepad.exe` as the command to create a tile that opens Notepad.

### Adding a toggle tile

Select `Toggle` when a tile represents an on/off state. A toggle requires three commands:

- **State command:** Checks the current state. It must exit with code `0` when active and any non-zero code when inactive.
- **Enable command:** Runs when the inactive tile is clicked.
- **Disable command:** Runs when the active tile is clicked.

State commands always run hidden so they cannot steal focus from the tile canvas. Enable and disable commands can be configured to run hidden as well.

The bundled Proxy and Theme tiles are examples of toggle configurations.

### Managing tiles

Click the gear button and open the **Tiles** tab. From there you can:

- Clear **Show** to hide a tile without deleting its configuration.
- Restore a hidden tile by selecting **Show** again.
- Move tiles up or down to change their canvas order.
- Delete tiles permanently when settings are saved.

Changes are applied only when you click **Save**. Closing the window with **Cancel** leaves the existing configuration untouched.

### Application settings

The **General** tab contains these settings:

- **Start timActionCenter when I sign in to Windows:** Adds or removes the application from the current user's Windows startup registry entry.
- **Hide the tile canvas when it loses focus:** Controls the action-center-style auto-hide behavior.
- **Keep the tile canvas above other windows:** Controls the window's always-on-top behavior.
- **Ask before deleting a tile:** Enables confirmation inside the tile manager.

### Command variables

Commands, arguments, and working directories support normal Windows environment variables such as `%USERPROFILE%` and these application variables:

| Variable | Resolves to |
| --- | --- |
| `{AppDirectory}` | Directory containing `timActionCenter.exe` |
| `{LocalAppData}` | Current user's local application-data directory |
| `{AppData}` | Current user's roaming application-data directory |

### Configuration file

The writable user configuration is stored at:

```text
%LOCALAPPDATA%\timActionCenter\tiles.json
```

Application preferences are stored separately at:

```text
%LOCALAPPDATA%\timActionCenter\settings.json
```

The file is created from `Configuration\default-tiles.json` on first launch. Tiles added through the application are saved automatically.

You may edit `tiles.json` manually while timActionCenter is not running. Keep a backup before editing it because malformed JSON cannot be loaded. If loading fails, the application reports the error and displays the packaged defaults for that session.

To reset the application, stop timActionCenter, delete the user `tiles.json`, and start the application again. A new copy of the packaged defaults will be created.

### Security

Tiles execute commands with the same permissions as the current user. Do not import configuration files or commands from sources you do not trust.

## Configuration reference

The configuration document contains a version and an ordered tile collection:

```json
{
  "version": 1,
  "tiles": []
}
```

### Tile fields

| Field | Required | Description |
| --- | --- | --- |
| `id` | Yes | Unique GUID for the tile |
| `title` | Yes | Display title |
| `description` | No | Tooltip and accessibility description |
| `iconGlyph` | Yes | Segoe Fluent Icons character, normally encoded as `\uE71D` |
| `behavior` | Yes | `Action` or `Toggle` |
| `isVisible` | No | Whether the tile appears on the canvas; defaults to `true` |
| `command` | For `Action` | Command executed by an action tile |
| `stateCommand` | For `Toggle` | Returns exit code `0` when active |
| `enableCommand` | For `Toggle` | Activates the toggle |
| `disableCommand` | For `Toggle` | Deactivates the toggle |

### Command fields

| Field | Required | Description |
| --- | --- | --- |
| `fileName` | Yes | Executable, script host, file, or shell target |
| `arguments` | No | Raw command-line arguments |
| `workingDirectory` | No | Process working directory |
| `windowMode` | No | `Normal` or `Hidden`; defaults to `Normal` |
| `waitForExit` | No | Wait for the process before re-enabling the tile; defaults to `false` |

### Action example

```json
{
  "id": "ad6ffb5e-9122-4a09-88ea-c5846b3cc440",
  "title": "Notepad",
  "description": "Open Notepad",
  "iconGlyph": "\uE756",
  "behavior": "Action",
  "command": {
    "fileName": "notepad.exe",
    "arguments": "",
    "windowMode": "Normal",
    "waitForExit": false
  }
}
```

See [the packaged defaults](timActionCenter/Configuration/default-tiles.json) for complete toggle examples.

## Developer guide

### Prerequisites

- Windows 10 or Windows 11
- .NET 10 SDK
- Git

### Build and run

From the repository root:

```powershell
dotnet restore .\timActionCenter\timActionCenter.csproj
dotnet build .\timActionCenter\timActionCenter.csproj
dotnet run --project .\timActionCenter\timActionCenter.csproj
```

Create a Release build with:

```powershell
dotnet build .\timActionCenter\timActionCenter.csproj --configuration Release
```

The application starts in the tray. Stop the running instance before rebuilding if Windows reports that `timActionCenter.exe` is locked.

### Architecture

The project keeps presentation, persistence, and execution separate:

```text
timActionCenter/
├── Configuration/       Packaged default tile JSON
├── Infrastructure/      Reusable command infrastructure
├── Models/              Configuration records and enums
├── Presentation/        Icon provider and XAML converters
├── Services/            JSON storage, process execution, tray icon
├── ViewModels/           Runtime tile state and command coordination
├── AddTileWindow.*       Tile creation dialog
├── SettingsWindow.*      Application and tile management dialog
└── MainWindow.*          Tile canvas and window lifecycle
```

The main runtime flow is:

1. `TileConfigurationStore` copies the packaged defaults on first launch and loads the user JSON.
2. `MainWindow` creates one `TileViewModel` for each configuration entry.
3. XAML renders the observable tile collection through one reusable data template.
4. `TileViewModel` selects the configured action, enable, or disable command.
5. `TileCommandRunner` expands variables and launches the process.
6. Toggle state commands are executed when the canvas is activated so visual state stays current.

### Important components

- `Models/TileConfiguration.cs` defines serializable tile data.
- `Models/TileCommand.cs` defines a generic process command.
- `Services/TileConfigurationStore.cs` validates and atomically saves JSON.
- `Services/TileCommandRunner.cs` is the only process-launching implementation.
- `Services/ApplicationSettingsStore.cs` persists application preferences.
- `Services/StartupService.cs` manages the current user's Windows startup entry.
- `ViewModels/TileViewModel.cs` contains generic action/toggle behavior.
- `Presentation/SegoeFluentIconProvider.cs` reads available glyphs from the bundled font at runtime.
- `Configuration/default-tiles.json` defines all default functionality.

### Adding functionality

Prefer adding a tile definition or external script instead of adding feature-specific C# code. A new Windows action should normally be expressible as:

- One command for an action tile, or
- State, enable, and disable commands for a toggle tile.

Only extend the C# model or command runner when the capability is genuinely generic and useful to many tile types. Do not add domain-specific enum values such as individual Windows features.

When extending the configuration format:

1. Update the model in `Models`.
2. Add validation in `TileConfigurationStore`.
3. Update the add-tile dialog when users need to edit the new field.
4. Update `default-tiles.json` and this README.
5. Increment the configuration version if the change is not backward-compatible.

### Icons

`Segoe Fluent Icons.ttf` is copied into the build output. `SegoeFluentIconProvider` reads its character map and exposes every private-use glyph to the icon picker. Tile configuration stores the selected glyph itself, so there is no icon enum or hard-coded glyph catalog.

### Default helper programs

The default Theme toggle currently invokes `ThemeSwitcher.exe` and `RefreshWallpaper.exe` through commands in `default-tiles.json`. The core application does not reference these helpers directly.

## Creating a release

Commit your changes, then run this single command from the repository root:

```powershell
.\release.ps1
```

The script reads the latest `vX.Y.Z` tag from `origin`, increments the patch number, builds the ZIP and checksum locally, pushes the current branch, and pushes the new tag. For example, `v0.2.2` becomes `v0.2.3`.

The tag starts the GitHub Actions workflow, which publishes the GitHub Release and attaches its ZIP and checksum. Follow its progress from the repository's **Actions** tab.

The working tree must be clean so a release always corresponds to committed source. The script stops with an explanation if files still need to be committed.

For an intentional minor or major version increase, use:

```powershell
.\release.ps1 -Bump Minor
.\release.ps1 -Bump Major
```

### Packaging without publishing

The repository includes `scripts/package-release.ps1`. It creates a self-contained, single-file Windows application, keeps required content files beside it, compresses everything into a ZIP, and generates a SHA-256 checksum.

Run this from the repository root:

```powershell
.\scripts\package-release.ps1 -Version 0.3.0 -Runtime win-x64
```

For Windows on ARM:

```powershell
.\scripts\package-release.ps1 -Version 0.3.0 -Runtime win-arm64
```

Generated files are placed under `artifacts\`:

```text
artifacts\timActionCenter-0.3.0-win-x64.zip
artifacts\timActionCenter-0.3.0-win-x64.zip.sha256
```

The ZIP contains `timActionCenter.exe` plus the configuration, font, icons, and helper executables required at runtime. Test the extracted package on a clean Windows machine before publishing it.

The generated executables are not code-signed. Public releases may trigger a Microsoft Defender SmartScreen warning until you sign them with an Authenticode certificate and build reputation. Keep signing certificates and passwords outside the repository, preferably in a protected CI secret store.

## GitHub release automation

The workflow at `.github\workflows\release.yml` runs on version tags matching `v*`. It:

1. Checks out the tagged source.
2. Installs the .NET 10 SDK.
3. Runs the same local packaging script for `win-x64`.
4. Stores the ZIP and checksum as workflow artifacts.
5. Creates a GitHub Release with generated release notes.
6. Attaches the ZIP and checksum to the release.

The workflow uses GitHub's temporary repository `GITHUB_TOKEN`; no personal access token is required. Its permission is limited to `contents: write` so it can create the release and upload assets. Normally, use `release.ps1` instead of creating and pushing release tags manually.

If GitHub reports `Resource not accessible by integration`, check the repository or organization Actions policy and confirm workflows may receive write access. The workflow already declares the required `contents: write` permission.

Relevant GitHub documentation:

- [Building and testing .NET with GitHub Actions](https://docs.github.com/en/actions/tutorials/build-and-test-code/net)
- [Using the automatic GITHUB_TOKEN](https://docs.github.com/en/actions/concepts/security/github_token)
- [Managing GitHub releases](https://docs.github.com/en/repositories/releasing-projects-on-github/managing-releases-in-a-repository?tool=cli)

## Project status

timActionCenter is under active development. Configuration files should be backed up before testing schema changes.
