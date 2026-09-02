namespace timActionCenter.Models;

public sealed class ApplicationSettings
{
    public bool StartWithWindows { get; set; }

    public bool HideOnDeactivation { get; set; } = true;

    public bool KeepWindowOnTop { get; set; } = true;

    public bool ConfirmBeforeDeletingTiles { get; set; } = true;

    public ApplicationSettings Clone()
    {
        return new ApplicationSettings
        {
            StartWithWindows = StartWithWindows,
            HideOnDeactivation = HideOnDeactivation,
            KeepWindowOnTop = KeepWindowOnTop,
            ConfirmBeforeDeletingTiles = ConfirmBeforeDeletingTiles,
        };
    }
}
