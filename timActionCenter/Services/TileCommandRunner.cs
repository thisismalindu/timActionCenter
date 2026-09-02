using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using timActionCenter.Models;

namespace timActionCenter.Services;

public sealed class TileCommandRunner
{
    public async Task RunAsync(TileCommand command)
    {
        using Process process = Start(command, forceHidden: false);

        if (command.WaitForExit)
        {
            await process.WaitForExitAsync();
        }
    }

    public async Task<bool> ReturnsSuccessAsync(TileCommand command)
    {
        using Process process = Start(command, forceHidden: true);
        await process.WaitForExitAsync();
        return process.ExitCode == 0;
    }

    private static Process Start(TileCommand command, bool forceHidden)
    {
        if (string.IsNullOrWhiteSpace(command.FileName))
        {
            throw new InvalidOperationException("The tile command has no file name.");
        }

        bool isHidden = forceHidden || command.WindowMode == CommandWindowMode.Hidden;
        var startInfo = new ProcessStartInfo
        {
            FileName = Expand(command.FileName),
            Arguments = Expand(command.Arguments),
            CreateNoWindow = isHidden,
            UseShellExecute = !isHidden,
            WindowStyle = isHidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
        };

        if (!string.IsNullOrWhiteSpace(command.WorkingDirectory))
        {
            startInfo.WorkingDirectory = Expand(command.WorkingDirectory);
        }

        return Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Unable to start '{startInfo.FileName}'.");
    }

    private static string Expand(string value)
    {
        string appDirectory = AppContext.BaseDirectory.TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar);

        return Environment.ExpandEnvironmentVariables(value)
            .Replace("{AppDirectory}", appDirectory, StringComparison.OrdinalIgnoreCase)
            .Replace(
                "{LocalAppData}",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                StringComparison.OrdinalIgnoreCase)
            .Replace(
                "{AppData}",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                StringComparison.OrdinalIgnoreCase);
    }
}
