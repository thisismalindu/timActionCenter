using System;

namespace timActionCenter.Models;

public sealed class TileConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string IconGlyph { get; set; } = string.Empty;

    public TileBehavior Behavior { get; set; } = TileBehavior.Action;

    public bool IsVisible { get; set; } = true;

    public TileCommand? Command { get; set; }

    public TileCommand? StateCommand { get; set; }

    public TileCommand? EnableCommand { get; set; }

    public TileCommand? DisableCommand { get; set; }

    public TileConfiguration Clone()
    {
        return new TileConfiguration
        {
            Id = Id,
            Title = Title,
            Description = Description,
            IconGlyph = IconGlyph,
            Behavior = Behavior,
            IsVisible = IsVisible,
            Command = Command?.Clone(),
            StateCommand = StateCommand?.Clone(),
            EnableCommand = EnableCommand?.Clone(),
            DisableCommand = DisableCommand?.Clone(),
        };
    }
}
