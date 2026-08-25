using System;
using System.IO;
using System.Text;
using System.Text.Json;
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

    // Dynamic config selection helper
    Func<string, string?> configProvider = EnvManager.ConfigProvider; // Default to public .env
    string jsonPath = Path.Combine(AppContext.BaseDirectory, "profiles.json");

    if (File.Exists(jsonPath))
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n--- NoTrace Pro Profile Manager ---");
        Console.ResetColor();

        using var jsonDoc = JsonDocument.Parse(File.ReadAllText(jsonPath));
        var profilesElement = jsonDoc.RootElement.GetProperty("Profiles");
        
        var profileList = new System.Collections.Generic.List<(string Name, string ApiId, string ApiHash, string Phone, string Session)>();
        foreach (var property in profilesElement.EnumerateObject())
        {
            profileList.Add((
                property.Name,
                property.Value.GetProperty("API_ID").GetString() ?? "",
                property.Value.GetProperty("API_HASH").GetString() ?? "",
                property.Value.GetProperty("PHONE_NUMBER").GetString() ?? "",
                property.Value.GetProperty("SESSION_NAME").GetString() ?? property.Name
            ));
        }

        // Display selection menu
        for (int i = 0; i < profileList.Count; i++)
        {
            Console.WriteLine($" [{i + 1}] {profileList[i].Name} ({profileList[i].Phone})");
        }
        Console.WriteLine($" [{profileList.Count + 1}] Fallback to default public .env file");

        Console.Write("\nSelect active runtime profile index: ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= profileList.Count)
        {
            var selected = profileList[choice - 1];
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[Pro] Loading active identity profile: {selected.Name.ToUpper()}");
            Console.ResetColor();

            // Override WTelegram config completely with JSON profile properties
            configProvider = configKey => configKey switch
            {
                "api_id" => selected.ApiId,
                "api_hash" => selected.ApiHash,
                "phone_number" => selected.Phone,
                "session_pathname" => Path.Combine(AppContext.BaseDirectory, $"{selected.Session}.session"),
                _ => null
            };
        }
        else
        {
            Console.WriteLine("\n[Note] Standard environment fallback engine selected.");
        }
    }

    // Initialize WTelegram client using selected provider rules
    using var client = new Client(configProvider);
    var user = await client.LoginUserIfNeeded();
    
    Console.WriteLine(LocaleManager.T(TextKey.LoginSuccess, user.first_name, user.id));

    // Pass private premium service contract into the public menu orchestrator
    ICleanupService proCleanupService = new TurboCleanupService(client);
    var menuController = new MenuController(client, proCleanupService);

    await menuController.StartEngineAsync();
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