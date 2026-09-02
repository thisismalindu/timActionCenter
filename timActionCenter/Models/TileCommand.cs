namespace timActionCenter.Models;

public sealed class TileCommand
{
    public string FileName { get; set; } = string.Empty;

    public string Arguments { get; set; } = string.Empty;

    public string WorkingDirectory { get; set; } = string.Empty;

    public CommandWindowMode WindowMode { get; set; } = CommandWindowMode.Normal;

    public bool WaitForExit { get; set; }

    public TileCommand Clone()
    {
        return new TileCommand
        {
            FileName = FileName,
            Arguments = Arguments,
            WorkingDirectory = WorkingDirectory,
            WindowMode = WindowMode,
            WaitForExit = WaitForExit,
        };
    }
}
