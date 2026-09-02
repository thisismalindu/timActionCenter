using System;
using System.IO;
using System.Text.Json;
using timActionCenter.Models;

namespace timActionCenter.Services;

public sealed class ApplicationSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public ApplicationSettingsStore()
    {
        string configurationDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "timActionCenter");

        Directory.CreateDirectory(configurationDirectory);
        SettingsPath = Path.Combine(configurationDirectory, "settings.json");
    }

    public string SettingsPath { get; }

    public ApplicationSettings Load()
    {
        if (!File.Exists(SettingsPath))
        {
            var defaults = new ApplicationSettings();
            Save(defaults);
            return defaults;
        }

        string json = File.ReadAllText(SettingsPath);
        return JsonSerializer.Deserialize<ApplicationSettings>(json, JsonOptions)
            ?? throw new InvalidDataException("The application settings file is empty.");
    }

    public void Save(ApplicationSettings settings)
    {
        string json = JsonSerializer.Serialize(settings, JsonOptions);
        string temporaryPath = SettingsPath + ".tmp";

        File.WriteAllText(temporaryPath, json);
        File.Move(temporaryPath, SettingsPath, overwrite: true);
    }
}
