using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrackYourMeals.Data;

namespace TrackYourMeals.Services;

public sealed class SettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public SettingsService(string? settingsPath = null)
    {
        SettingsPath = settingsPath ?? GetDefaultSettingsPath();
    }

    public string SettingsPath { get; }

    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            // Preferências corrompidas ou inacessíveis não devem impedir o app de abrir.
            return new AppSettings();
        }
    }

    public void Save(AppSettings settings)
    {
        var directory = Path.GetDirectoryName(SettingsPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings, JsonOptions));
    }

    public static string GetDefaultSettingsPath()
    {
        var dataDirectory = Path.GetDirectoryName(TrackYourMealsDbContextFactory.GetDefaultDatabasePath())!;
        return Path.Combine(dataDirectory, "settings.json");
    }
}
