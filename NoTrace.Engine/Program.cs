using System;
using System.Text;
using System.Threading.Tasks;
using WTelegram;
using NoTrace.Engine.Configuration; // Public core configuration
using NoTrace.Engine.Core;          // Public core interface
using NoTrace.Engine.UI;            // Public core UI menus
using NoTrace.Engine.Modules;

try
{
    Console.OutputEncoding = Encoding.UTF8;
    Console.InputEncoding = Encoding.UTF8;

    WTelegram.Helpers.Log = (lvl, str) =>
    {
        if (lvl >= 3)
        {
            Console.WriteLine($"\n[Telegram API Warning] {str}");
        }
    };

    Console.WriteLine(LocaleManager.T(TextKey.EngineInitializing));

    // Restore saved language preference, if any.
    var appSettings = SessionConfig.LoadSettings();
    if (Enum.TryParse<Language>(appSettings.Language, out var savedLanguage))
    {
        LocaleManager.CurrentLanguage = savedLanguage;
    }

    // Outer loop: lets Profile Management > Switch/Add tear down the current
    // Client and restart the login flow in-process, without relaunching the exe.
    bool keepRunning = true;
    bool forcePicker = false; // true only when re-entering because the user asked to switch/add a profile

    while (keepRunning)
    {
        appSettings = SessionConfig.LoadSettings(); // re-read in case credentials changed mid-session
        var profiles = SessionConfig.LoadProfiles();

        UserProfile? selectedProfile = null;
        bool isNewProfile;

        if (profiles.Count == 0)
        {
            // First run: nothing saved yet, go straight into a fresh login.
            Console.WriteLine(LocaleManager.T(TextKey.FirstRunWelcome));
            isNewProfile = true;
        }
        else if (profiles.Count == 1 && !forcePicker)
        {
            // Normal startup with exactly one saved profile: skip the picker.
            // But if the user got here via "switch/add profile", always show the
            // picker so "Add new account" is reachable even with just one entry.
            selectedProfile = profiles[0];
            isNewProfile = false;
        }
        else
        {
            selectedProfile = MenuController.PromptProfilePicker(profiles);
            isNewProfile = selectedProfile == null;
        }

        forcePicker = false; // reset; only re-armed below if the user switches again

        string sessionName = selectedProfile?.SessionName ?? $"session_{Guid.NewGuid():N}";

        Func<string, string?> configProvider = configKey => configKey switch
        {
            "api_id" => ResolveApiId(appSettings),
            "api_hash" => ResolveApiHash(appSettings),
            "phone_number" => selectedProfile?.PhoneNumber ?? PromptPhoneNumber(),
            "session_pathname" => SessionConfig.GetSessionPath(sessionName),
            "verification_code" => PromptVerificationCode(),
            "password" => PromptPassword(), // used transiently only, never persisted
            _ => null
        };

        using var client = new Client(configProvider);
        var user = await client.LoginUserIfNeeded();

        Console.WriteLine(LocaleManager.T(TextKey.LoginSuccess, user.first_name, user.id));

        if (isNewProfile)
        {
            var newProfile = new UserProfile
            {
                PhoneNumber = user.phone ?? selectedProfile?.PhoneNumber ?? "",
                DisplayName = $"{user.first_name} {user.last_name}".Trim(),
                Username = user.username ?? "",
                SessionName = sessionName
            };

            profiles.Add(newProfile);
            SessionConfig.SaveProfiles(profiles);
        }

        // Pass private premium service contract into the public menu orchestrator
        ICleanupService proCleanupService = new TurboCleanupService(client);
        var menuController = new MenuController(client, proCleanupService);

        bool switchProfileRequested = await menuController.StartEngineAsync();
        keepRunning = switchProfileRequested;
        forcePicker = switchProfileRequested;
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n[Fatal Pro Runtime] Engine initialization aborted: {ex.Message}");
    Console.ResetColor();
}
finally
{
    Console.WriteLine(LocaleManager.T(TextKey.EngineShutdown));
}

static string ResolveApiId(AppSettings settings)
{
    if (settings.UseOwnApiCredentials)
        return settings.CustomApiId;

    if (EmbeddedCredentials.IsAvailable)
        return EmbeddedCredentials.ApiId;

    // Self-built/cloned binary with no embedded credentials and no custom
    // override set yet — fall back to the .env flow for backward compatibility.
    return EnvManager.ConfigProvider("api_id") ?? "";
}

static string ResolveApiHash(AppSettings settings)
{
    if (settings.UseOwnApiCredentials)
        return settings.CustomApiHash;

    if (EmbeddedCredentials.IsAvailable)
        return EmbeddedCredentials.ApiHash;

    return EnvManager.ConfigProvider("api_hash") ?? "";
}

static string PromptPhoneNumber()
{
    Console.Write(LocaleManager.T(TextKey.PhoneNumberPrompt));
    return Console.ReadLine() ?? "";
}

static string PromptVerificationCode()
{
    Console.Write("Enter verification challenge code: ");
    return Console.ReadLine() ?? "";
}

static string PromptPassword()
{
    Console.Write("Enter secondary account security token (2FA): ");
    return Console.ReadLine() ?? "";
}
