using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NoTrace.Pro.Engine;

public class AppSettings
{
    public string Language { get; set; } = "EN";
    public string LicenseKey { get; set; } = "";
    public string LastHWID { get; set; } = "";
    public bool IsPremium { get; set; } = false;
}

public class UserProfile
{
    public string PhoneNumber { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Username { get; set; } = "";
    public string SessionName { get; set; } = "";
}

public static class SessionConfig
{
    private static readonly string AppDataDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "NoTrace");

    private static readonly string SettingsFile = Path.Combine(AppDataDir, "settings.json");
    private static readonly string ProfilesFile = Path.Combine(AppDataDir, "profiles.json");

    static SessionConfig()
    {
        try
        {
            if (!Directory.Exists(AppDataDir))
            {
                Directory.CreateDirectory(AppDataDir);
            }
        }
        catch { }
    }

    public static string GetSessionPath(string sessionName)
    {
        return Path.Combine(AppDataDir, $"{sessionName}.session");
    }

    public static AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsFile))
            {
                string json = File.ReadAllText(SettingsFile);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch { }
        return new AppSettings();
    }

    public static void SaveSettings(AppSettings settings)
    {
        try
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFile, json);
        }
        catch { }
    }

    public static List<UserProfile> LoadProfiles()
    {
        try
        {
            if (File.Exists(ProfilesFile))
            {
                string json = File.ReadAllText(ProfilesFile);
                return JsonSerializer.Deserialize<List<UserProfile>>(json) ?? new List<UserProfile>();
            }
        }
        catch { }
        return new List<UserProfile>();
    }

    public static void SaveProfiles(List<UserProfile> profiles)
    {
        try
        {
            string json = JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ProfilesFile, json);
        }
        catch { }
    }
}
