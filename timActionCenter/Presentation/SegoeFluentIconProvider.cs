using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace timActionCenter.Presentation;

public static class SegoeFluentIconProvider
{
    private const string FontFileName = "Segoe Fluent Icons.ttf";
    private const string FontFamilyName = "Segoe Fluent Icons";
    private const int FirstPrivateUseCodePoint = 0xE000;
    private const int LastPrivateUseCodePoint = 0xF8FF;
    private const int DefaultIconCodePoint = 0xE71D;

    static SegoeFluentIconProvider()
    {
        string fontPath = Path.Combine(AppContext.BaseDirectory, FontFileName);
        var glyphTypeface = new GlyphTypeface(new Uri(fontPath));

        FontFamily = new FontFamily(
            new Uri(AppContext.BaseDirectory, UriKind.Absolute),
            $"./#{FontFamilyName}");

        Icons = glyphTypeface.CharacterToGlyphMap.Keys
            .Where(IsPrivateUseCodePoint)
            .OrderBy(codePoint => codePoint)
            .Select(codePoint => new IconGlyphOption(codePoint, FontFamily))
            .ToArray();

        DefaultIcon = Icons.FirstOrDefault(icon => icon.CodePoint == DefaultIconCodePoint)
            ?? Icons[0];
    }

    public static FontFamily FontFamily { get; }

    public static IReadOnlyList<IconGlyphOption> Icons { get; }

    public static IconGlyphOption DefaultIcon { get; }

    private static bool IsPrivateUseCodePoint(int codePoint)
    {
        return codePoint >= FirstPrivateUseCodePoint && codePoint <= LastPrivateUseCodePoint;
    }
}

public sealed class IconGlyphOption
{
    public IconGlyphOption(int codePoint, FontFamily fontFamily)
    {
        CodePoint = codePoint;
        FontFamily = fontFamily;
        Glyph = char.ConvertFromUtf32(codePoint);
    }

    public int CodePoint { get; }

    public string Code => $"U+{CodePoint:X4}";

    public string Glyph { get; }

    public FontFamily FontFamily { get; }
}
