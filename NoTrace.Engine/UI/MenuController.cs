using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TL;
using WTelegram;
using NoTrace.Engine.Core; // UserCanceledException lives here

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

    public async Task StartEngineAsync()
    {
        while (true)
        {
            Console.WriteLine($"\n{LocaleManager.T(TextKey.MainMenuTitle)}");
            Console.WriteLine(LocaleManager.T(TextKey.MenuOption1));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOption2));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOption3));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOption4));
            Console.WriteLine(LocaleManager.T(TextKey.MenuOption0));
            Console.Write($"\n{LocaleManager.T(TextKey.SelectAction)}");

            string mainMenuChoice = Console.ReadLine() ?? "";

            if (mainMenuChoice == "0")
                break;

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
            Console.WriteLine(LocaleManager.T(TextKey.SelectLanguageOption));
            Console.Write("\nChoice: ");

            string choice = Console.ReadLine() ?? "";
            if (choice == "0")
                break;

            switch (choice)
            {
                case "1":
                    LocaleManager.CurrentLanguage = Language.UZ;
                    Console.WriteLine($"\n[SUCCESS] {LocaleManager.T(TextKey.LanguageChanged)}");
                    return;
                case "2":
                    LocaleManager.CurrentLanguage = Language.EN;
                    Console.WriteLine($"\n[SUCCESS] {LocaleManager.T(TextKey.LanguageChanged)}");
                    return;
                case "3":
                    LocaleManager.CurrentLanguage = Language.RU;
                    Console.WriteLine($"\n[SUCCESS] {LocaleManager.T(TextKey.LanguageChanged)}");
                    return;
                default:
                    Console.WriteLine(LocaleManager.T(TextKey.InvalidSelection));
                    break;
            }
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
                    Console.Write(LocaleManager.T(confirmKey));
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
                Console.WriteLine(LocaleManager.T(TextKey.OperationComplete));
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

        Console.Write(LocaleManager.T(TextKey.ConfirmBlocklistPurge));
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
