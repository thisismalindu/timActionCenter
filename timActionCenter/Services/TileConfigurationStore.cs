using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using timActionCenter.Models;

namespace timActionCenter.Services;

public sealed class TileConfigurationStore
{
    private const int CurrentVersion = 1;
    private const string ConfigurationFileName = "tiles.json";

    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private readonly string _defaultConfigurationPath = Path.Combine(
        AppContext.BaseDirectory,
        "Configuration",
        "default-tiles.json");

    public TileConfigurationStore()
    {
        string configurationDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "timActionCenter");

        Directory.CreateDirectory(configurationDirectory);
        ConfigurationPath = Path.Combine(configurationDirectory, ConfigurationFileName);
    }

    public string ConfigurationPath { get; }

    public List<TileConfiguration> Load()
    {
        if (!File.Exists(ConfigurationPath))
        {
            File.Copy(_defaultConfigurationPath, ConfigurationPath);
        }

        return Read(ConfigurationPath);
    }

    public List<TileConfiguration> LoadDefaults() => Read(_defaultConfigurationPath);

    public void Save(IEnumerable<TileConfiguration> tiles)
    {
        var document = new TileConfigurationDocument
        {
            Version = CurrentVersion,
            Tiles = tiles.ToList(),
        };

        string json = JsonSerializer.Serialize(document, JsonOptions);
        string temporaryPath = ConfigurationPath + ".tmp";

        File.WriteAllText(temporaryPath, json);
        File.Move(temporaryPath, ConfigurationPath, overwrite: true);
    }

    private static List<TileConfiguration> Read(string path)
    {
        string json = File.ReadAllText(path);
        TileConfigurationDocument document = JsonSerializer.Deserialize<TileConfigurationDocument>(
            json,
            JsonOptions)
            ?? throw new InvalidDataException("The tile configuration is empty.");

        if (document.Version != CurrentVersion)
        {
            throw new InvalidDataException(
                $"Unsupported tile configuration version: {document.Version}.");
        }

        foreach (TileConfiguration tile in document.Tiles)
        {
            Validate(tile);
        }

        return document.Tiles;
    }

    private static void Validate(TileConfiguration tile)
    {
        if (tile.Id == Guid.Empty)
        {
            throw new InvalidDataException("Every tile must have an ID.");
        }

        if (string.IsNullOrWhiteSpace(tile.Title))
        {
            throw new InvalidDataException($"Tile '{tile.Id}' must have a title.");
        }

        if (string.IsNullOrEmpty(tile.IconGlyph))
        {
            throw new InvalidDataException($"Tile '{tile.Title}' must have an icon glyph.");
        }

        if (tile.Behavior == TileBehavior.Action)
        {
            ValidateCommand(tile.Command, tile.Title, "command");
            return;
        }

        ValidateCommand(tile.StateCommand, tile.Title, "state command");
        ValidateCommand(tile.EnableCommand, tile.Title, "enable command");
        ValidateCommand(tile.DisableCommand, tile.Title, "disable command");
    }

    private static void ValidateCommand(TileCommand? command, string title, string commandName)
    {
        if (command is null || string.IsNullOrWhiteSpace(command.FileName))
        {
            throw new InvalidDataException($"Tile '{title}' must specify a {commandName}.");
        }
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };

        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    private sealed class TileConfigurationDocument
    {
        public int Version { get; set; } = CurrentVersion;

        public List<TileConfiguration> Tiles { get; set; } = new();
    }
}
