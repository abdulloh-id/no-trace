using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TL;
using WTelegram;
using NoTrace.Engine.Core; // UserCanceledException lives here
using NoTrace.Engine.Configuration; // SessionConfig, AppSettings, UserProfile, EmbeddedCredentials

namespace NoTrace.Engine.UI;

/// <summary>
/// Manages the primary terminal interaction loops and handles user input routing.
/// </summary>
public class MenuController
{
    private readonly Client _client;
    private readonly ICleanupService _cleanupService;

    public MenuController(Client client, ICleanupService cleanupService)
    {
        _client = client;
        _cleanupService = cleanupService;
    }

    /// <summary>
    /// Runs the main menu loop. Returns true if the user requested a profile switch
    /// from Profile Management (caller should tear down the Client and restart the
    /// login flow), or false on a normal exit via [0] at the main menu.
    /// </summary>
    public async Task<bool> StartEngineAsync()
    {
        while (true)
        {
            Console.WriteLine($"\n{LocaleManager.T(TextKey.MainMenuTitle)}");
            Console.WriteLine(LocaleManager.T(TextKey.MenuOption1));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOption2));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOption3));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOption4));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOption5));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOption0));
            Console.Write($"\n{LocaleManager.T(TextKey.SelectAction)}");

            string mainMenuChoice = Console.ReadLine() ?? "";

            if (mainMenuChoice == "0")
                return false;

            switch (mainMenuChoice)
            {
                case "1":
                    await HandleActiveChatOperationsAsync();
                    break;
                case "2":
                    await HandleBlocklistManagerAsync();
                    break;
                case "3":
                    try
                    {
                        await _cleanupService.PurgeUselessContactsAsync();
                    }
                    catch (UserCanceledException)
                    {
                        Console.WriteLine(LocaleManager.T(TextKey.OperationCancelled));
                    }
                    break;
                case "4":
                    if (HandleManageProfilesMenu())
                        return true; // caller tears down Client and restarts login flow
                    break;
                case "5":
                    HandleSettingsMenu();
                    break;
                default:
                    Console.WriteLine(LocaleManager.T(TextKey.InvalidSelection));
                    break;
            }
        }
    }

    private void HandleSettingsMenu()
    {
        while (true)
        {
            Console.WriteLine($"\n{LocaleManager.T(TextKey.SettingsMenuTitle)}");
            Console.WriteLine(LocaleManager.T(TextKey.SettingsOptLanguage));
            Console.WriteLine(LocaleManager.T(TextKey.SettingsOptApiCredentials));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOptBackToMain));
            Console.Write("\nChoice: ");

            string choice = Console.ReadLine() ?? "";
            if (choice == "0")
                return;

            switch (choice)
            {
                case "1":
                    HandleLanguageMenu();
                    break;
                case "2":
                    HandleApiCredentialsMenu();
                    break;
                default:
                    Console.WriteLine(LocaleManager.T(TextKey.InvalidSelection));
                    break;
            }
        }
    }

    private void HandleLanguageMenu()
    {
        Console.WriteLine(LocaleManager.T(TextKey.LanguageMenuTitle));
        Console.WriteLine(LocaleManager.T(TextKey.SelectLanguageOption));
        Console.Write("\nChoice: ");

        string choice = Console.ReadLine() ?? "";

        switch (choice)
        {
            case "1":
                LocaleManager.CurrentLanguage = Language.UZ;
                PersistLanguage(Language.UZ);
                Console.WriteLine($"\n[SUCCESS] {LocaleManager.T(TextKey.LanguageChanged)}");
                break;
            case "2":
                LocaleManager.CurrentLanguage = Language.EN;
                PersistLanguage(Language.EN);
                Console.WriteLine($"\n[SUCCESS] {LocaleManager.T(TextKey.LanguageChanged)}");
                break;
            case "3":
                LocaleManager.CurrentLanguage = Language.RU;
                PersistLanguage(Language.RU);
                Console.WriteLine($"\n[SUCCESS] {LocaleManager.T(TextKey.LanguageChanged)}");
                break;
            case "0":
                break;
            default:
                Console.WriteLine(LocaleManager.T(TextKey.InvalidSelection));
                break;
        }
    }

    private static void PersistLanguage(Language lang)
    {
        var settings = SessionConfig.LoadSettings();
        settings.Language = lang.ToString();
        SessionConfig.SaveSettings(settings);
    }

    private void HandleApiCredentialsMenu()
    {
        while (true)
        {
            var settings = SessionConfig.LoadSettings();

            Console.WriteLine(LocaleManager.T(TextKey.ApiCredentialsMenuTitle));
            Console.WriteLine(settings.UseOwnApiCredentials
                ? LocaleManager.T(TextKey.ApiCredentialsCurrentCustom)
                : LocaleManager.T(TextKey.ApiCredentialsCurrentEmbedded));

            if (!settings.UseOwnApiCredentials && !EmbeddedCredentials.IsAvailable)
                Console.WriteLine(LocaleManager.T(TextKey.ApiCredentialsEmbeddedUnavailable));

            Console.WriteLine(LocaleManager.T(TextKey.ApiCredentialsOptUseOwn));
            Console.WriteLine(LocaleManager.T(TextKey.ApiCredentialsOptGetOwn));
            Console.WriteLine(LocaleManager.T(TextKey.ApiCredentialsOptRevertToEmbedded));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOptBack));
            Console.Write(LocaleManager.T(TextKey.ApiCredentialsChoicePrompt));

            string choice = Console.ReadLine()?.Trim() ?? "";

            switch (choice)
            {
                case "0":
                    return;
                case "1":
                    Console.Write(LocaleManager.T(TextKey.ApiCredentialsPromptApiId));
                    string apiId = Console.ReadLine()?.Trim() ?? "";
                    Console.Write(LocaleManager.T(TextKey.ApiCredentialsPromptApiHash));
                    string apiHash = Console.ReadLine()?.Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(apiId) || string.IsNullOrWhiteSpace(apiHash))
                    {
                        Console.WriteLine(LocaleManager.T(TextKey.InvalidSelection));
                        break;
                    }

                    settings.UseOwnApiCredentials = true;
                    settings.CustomApiId = apiId;
                    settings.CustomApiHash = apiHash;
                    SessionConfig.SaveSettings(settings);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(LocaleManager.T(TextKey.ApiCredentialsSaved));
                    Console.ResetColor();
                    break;
                case "2":
                    Console.WriteLine(LocaleManager.T(TextKey.ApiCredentialsInstructions));
                    break;
                case "3":
                    settings.UseOwnApiCredentials = false;
                    SessionConfig.SaveSettings(settings);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(LocaleManager.T(TextKey.ApiCredentialsReverted));
                    Console.ResetColor();
                    break;
                default:
                    Console.WriteLine(LocaleManager.T(TextKey.InvalidSelection));
                    break;
            }
        }
    }

    /// <summary>
    /// Returns true if the user chose to switch/add a profile (caller should tear
    /// down the current Client and restart the login flow), false otherwise.
    /// </summary>
    private bool HandleManageProfilesMenu()
    {
        while (true)
        {
            var profiles = SessionConfig.LoadProfiles();

            Console.WriteLine(LocaleManager.T(TextKey.ManageProfilesMenuTitle));

            if (profiles.Count == 0)
            {
                Console.WriteLine(LocaleManager.T(TextKey.ManageProfilesNoProfiles));
            }
            else
            {
                for (int i = 0; i < profiles.Count; i++)
                {
                    string label = string.IsNullOrWhiteSpace(profiles[i].DisplayName)
                        ? profiles[i].PhoneNumber
                        : profiles[i].DisplayName;
                    Console.WriteLine(LocaleManager.T(TextKey.ManageProfilesEntry, i + 1, label, profiles[i].PhoneNumber));
                }
            }

            Console.WriteLine(new string('-', 30));
            Console.WriteLine(LocaleManager.T(TextKey.ManageProfilesOptRemove));
            Console.WriteLine(LocaleManager.T(TextKey.ManageProfilesOptSwitch));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOptBackToMain));
            Console.Write(LocaleManager.T(TextKey.ManageProfilesChoicePrompt));

            string choice = Console.ReadLine()?.Trim().ToUpperInvariant() ?? "";

            switch (choice)
            {
                case "0":
                    return false;

                case "R":
                    if (profiles.Count == 0)
                    {
                        Console.WriteLine(LocaleManager.T(TextKey.ManageProfilesNoProfiles));
                        break;
                    }

                    Console.Write(LocaleManager.T(TextKey.ManageProfilesRemoveChoicePrompt));
                    string removeInput = Console.ReadLine()?.Trim() ?? "";

                    if (!int.TryParse(removeInput, out int removeIndex) || removeIndex < 1 || removeIndex > profiles.Count)
                    {
                        Console.WriteLine(LocaleManager.T(TextKey.InvalidSelection));
                        break;
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(LocaleManager.T(TextKey.ManageProfilesRemoveConfirm));
                    Console.ResetColor();
                    string removeConfirm = Console.ReadLine()?.Trim().ToLower() ?? "";

                    if (removeConfirm != "y")
                    {
                        Console.WriteLine(LocaleManager.T(TextKey.OperationCancelled));
                        break;
                    }

                    var target = profiles[removeIndex - 1];
                    profiles.RemoveAt(removeIndex - 1);
                    SessionConfig.SaveProfiles(profiles);

                    try
                    {
                        string sessionPath = SessionConfig.GetSessionPath(target.SessionName);
                        if (System.IO.File.Exists(sessionPath))
                            System.IO.File.Delete(sessionPath);
                    }
                    catch { /* best-effort cleanup, missing/locked session file is not fatal */ }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(LocaleManager.T(TextKey.ManageProfilesRemoved));
                    Console.ResetColor();
                    break;

                case "S":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(LocaleManager.T(TextKey.ManageProfilesSwitchConfirm));
                    Console.ResetColor();
                    string switchConfirm = Console.ReadLine()?.Trim().ToLower() ?? "";

                    if (switchConfirm == "y")
                        return true;

                    Console.WriteLine(LocaleManager.T(TextKey.OperationCancelled));
                    break;

                default:
                    Console.WriteLine(LocaleManager.T(TextKey.InvalidSelection));
                    break;
            }
        }
    }

    /// <summary>
    /// Prompts the user to pick a saved profile, or add a new account. Called from
    /// Program.cs before the Client is constructed. Returns null if the user chose
    /// to add a new account (caller should proceed with a fresh login flow).
    /// </summary>
    public static UserProfile? PromptProfilePicker(List<UserProfile> profiles)
    {
        while (true)
        {
            Console.WriteLine(LocaleManager.T(TextKey.ProfilePickerTitle));

            for (int i = 0; i < profiles.Count; i++)
            {
                string label = string.IsNullOrWhiteSpace(profiles[i].DisplayName)
                    ? profiles[i].PhoneNumber
                    : profiles[i].DisplayName;
                Console.WriteLine(LocaleManager.T(TextKey.ProfilePickerEntry, i + 1, label, profiles[i].PhoneNumber));
            }

            Console.WriteLine(LocaleManager.T(TextKey.ProfilePickerAddNew, profiles.Count + 1));
            Console.Write(LocaleManager.T(TextKey.ProfilePickerChoicePrompt));

            string input = Console.ReadLine()?.Trim() ?? "";

            if (!int.TryParse(input, out int choice) || choice < 1 || choice > profiles.Count + 1)
            {
                Console.WriteLine(LocaleManager.T(TextKey.ProfilePickerInvalid));
                continue;
            }

            if (choice == profiles.Count + 1)
                return null; // "Add new account"

            return profiles[choice - 1];
        }
    }

    private async Task HandleActiveChatOperationsAsync()
    {
        while (true)
        {
            Console.WriteLine(LocaleManager.T(TextKey.FetchingActiveChats));
            var dialogs = await _client.Messages_GetAllDialogs();
            var allTargets = new List<IPeerInfo>();

            foreach (var chat in dialogs.chats.Values)
            {
                if (chat.IsActive)
                    allTargets.Add(chat);
            }

            foreach (var u in dialogs.users.Values)
            {
                if (u.IsBot || dialogs.dialogs.Any(d => d.Peer.ID == u.ID))
                    allTargets.Add(u);
            }

            Console.WriteLine("\n==========================================");

            Console.Write(LocaleManager.T(TextKey.SearchPrompt));
            string searchTerm = Console.ReadLine()?.ToLower() ?? "";

            if (searchTerm == "0")
                break;

            var filteredTargets = allTargets.Where(t =>
            {
                string title = t is ChatBase cb ? cb.Title : (t is User u2 ? $"{u2.first_name} {u2.last_name}" : LocaleManager.T(TextKey.FallbackUnknown));
                return title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
            }).ToList();

            if (filteredTargets.Count == 0)
            {
                Console.WriteLine(LocaleManager.T(TextKey.NoMatchesFound));
                continue;
            }

            Console.WriteLine(LocaleManager.T(TextKey.MatchingResultsHeader, filteredTargets.Count));

            for (int i = 0; i < Math.Min(20, filteredTargets.Count); i++)
            {
                var peer = filteredTargets[i];

                string typePrefix = peer is User usr
                    ? (usr.IsBot ? LocaleManager.T(TextKey.PrefixBot) : LocaleManager.T(TextKey.PrefixUser))
                    : LocaleManager.T(TextKey.PrefixChatChannel);

                string title = peer is ChatBase cb
                    ? cb.Title
                    : (peer is User u3 ? $"{u3.first_name} {u3.last_name} (@{u3.username})" : LocaleManager.T(TextKey.FallbackUnknown));

                Console.WriteLine($"[{i + 1}] {typePrefix,-14}{LocaleManager.T(TextKey.LabelTitleHeader)}{title}");
            }

            Console.Write(LocaleManager.T(TextKey.EnterIndexPrompt));
            string choiceInput = Console.ReadLine() ?? "";

            if (choiceInput == "0")
                continue;

            if (!int.TryParse(choiceInput, out int displayIndex) || displayIndex < 1 || displayIndex > filteredTargets.Count)
            {
                Console.WriteLine(LocaleManager.T(TextKey.InvalidSelectionAlert));
                continue;
            }

            int actualIndex = displayIndex - 1;
            var target = filteredTargets[actualIndex];
            var inputTarget = target.ToInputPeer();

            Console.WriteLine(LocaleManager.T(TextKey.ScanningMetadata));
            int[] myMessageIds = await _cleanupService.ScanMyMessageIdsAsync(inputTarget);
            Console.WriteLine(LocaleManager.T(TextKey.FoundMessagesCount, myMessageIds.Length));

            Console.WriteLine(LocaleManager.T(TextKey.SelectLevelTitle));
            Console.WriteLine(LocaleManager.T(TextKey.Level1Desc));
            Console.WriteLine(LocaleManager.T(TextKey.Level2Desc));
            Console.WriteLine(LocaleManager.T(TextKey.Level3Desc));
            Console.WriteLine(LocaleManager.T(TextKey.Level4Desc));
            Console.WriteLine(LocaleManager.T(TextKey.Level0Desc));

            Console.Write(LocaleManager.T(TextKey.ChooseLevelPrompt));
            string levelChoice = Console.ReadLine() ?? "";

            if (levelChoice == "0")
                continue;

            if (!new[] { "1", "2", "3", "4" }.Contains(levelChoice))
            {
                Console.WriteLine(LocaleManager.T(TextKey.InvalidLevelSelected));
                await Task.Delay(1000);
                continue;
            }

            int? targetLimit = null;
            if (levelChoice == "1")
            {
                targetLimit = PromptSurgicalDepth();
                if (targetLimit == -1) // sentinel: user pressed [0] at depth menu
                    continue;
            }

            // Confirmation gate for destructive levels
            if (levelChoice == "2" || levelChoice == "3" || levelChoice == "4")
            {
                TextKey confirmKey = levelChoice switch
                {
                    "2" => TextKey.ConfirmLevel2,
                    "3" => TextKey.ConfirmLevel3,
                    _   => TextKey.ConfirmLevel4
                };

                if (levelChoice == "4")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(LocaleManager.T(confirmKey));
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(LocaleManager.T(confirmKey));
                    Console.ResetColor();
                }

                string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
                if (confirm != "y")
                {
                    Console.WriteLine(LocaleManager.T(TextKey.OperationCancelled));
                    continue;
                }
            }

            Console.WriteLine(LocaleManager.T(TextKey.LaunchingWipe, levelChoice));

            try
            {
                await _cleanupService.ExecuteChatWipeAsync(target, levelChoice, myMessageIds, targetLimit);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(LocaleManager.T(TextKey.OperationComplete));
                Console.ResetColor();
            }
            catch (UserCanceledException)
            {
                Console.WriteLine(LocaleManager.T(TextKey.OperationCancelled));
            }

            await Task.Delay(1500);
        }
    }

    private async Task HandleBlocklistManagerAsync()
    {
        Console.WriteLine(LocaleManager.T(TextKey.BlocklistMenuTitle));
        Console.WriteLine(LocaleManager.T(TextKey.BlocklistOpt1));
        Console.WriteLine(LocaleManager.T(TextKey.BlocklistOpt2));
        Console.WriteLine(LocaleManager.T(TextKey.BlocklistOpt3));
        Console.WriteLine(LocaleManager.T(TextKey.BlocklistOpt0));
        Console.Write(LocaleManager.T(TextKey.BlocklistChoicePrompt));
        string blockChoice = Console.ReadLine() ?? "";

        if (blockChoice == "0")
            return;

        if (!new[] { "1", "2", "3" }.Contains(blockChoice))
        {
            Console.WriteLine(LocaleManager.T(TextKey.InvalidSelection));
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(LocaleManager.T(TextKey.ConfirmBlocklistPurge));
        Console.ResetColor();
        string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
        if (confirm != "y")
        {
            Console.WriteLine(LocaleManager.T(TextKey.OperationCancelled));
            return;
        }

        try
        {
            await _cleanupService.PurgeBlocklistAsync(blockChoice);
        }
        catch (UserCanceledException)
        {
            Console.WriteLine(LocaleManager.T(TextKey.OperationCancelled));
        }
    }

    /// <summary>
    /// Returns the chosen depth limit, or -1 as a sentinel value when the user presses [0] to go back.
    /// </summary>
    private int? PromptSurgicalDepth()
    {
        while (true)
        {
            Console.WriteLine(LocaleManager.T(TextKey.SurgicalDepthMenuTitle));
            Console.WriteLine(LocaleManager.T(TextKey.SurgicalDepthOpt1));
            Console.WriteLine(LocaleManager.T(TextKey.SurgicalDepthOpt2));
            Console.WriteLine(LocaleManager.T(TextKey.SurgicalDepthOpt3));
            Console.WriteLine(LocaleManager.T(TextKey.SurgicalDepthOpt4));
            Console.WriteLine(LocaleManager.T(TextKey.SurgicalDepthOpt5));
            Console.WriteLine(LocaleManager.T(TextKey.SurgicalDepthOpt0));
            Console.Write($"\n{LocaleManager.T(TextKey.SurgicalDepthChoicePrompt)}");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "0":
                    return -1; // sentinel → caller will `continue` the outer loop
                case "1":
                    return 50;
                case "2":
                    return 100;
                case "3":
                    return 200;
                case "4":
                    Console.Write($"\n{LocaleManager.T(TextKey.CustomDepthPrompt)}");
                    string raw = Console.ReadLine() ?? "";
                    if (raw == "0")
                        return -1; // [0] at custom input → also go back
                    if (int.TryParse(raw, out int customVal) && customVal > 0)
                        return customVal;
                    Console.WriteLine(LocaleManager.T(TextKey.InvalidDepthInput));
                    return null;
                case "5":
                    return null;
                default:
                    Console.WriteLine(LocaleManager.T(TextKey.InvalidSelection));
                    continue;
            }
        }
    }
}