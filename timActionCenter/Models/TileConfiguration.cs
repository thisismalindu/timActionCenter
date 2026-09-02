using System;

namespace timActionCenter.Models;

public sealed class TileConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string IconGlyph { get; set; } = string.Empty;

    public TileBehavior Behavior { get; set; } = TileBehavior.Action;

    public TileCommand? Command { get; set; }

    public TileCommand? StateCommand { get; set; }

    public TileCommand? EnableCommand { get; set; }

    public TileCommand? DisableCommand { get; set; }
}
