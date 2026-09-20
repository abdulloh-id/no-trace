using System;
using System.Collections.Generic;

namespace NoTrace.Engine.Core;

public enum Language
{
    UZ,
    EN,
    RU
}

public enum TextKey
{
    // Main Menu
    MainMenuTitle,
    MenuOption1,
    MenuOption2,
    MenuOption3,
    MenuOption4,
    MenuOption5,
    MenuOption0,
    MenuOptBackToMain,
    MenuOptBack,
    SelectAction,
    InvalidSelection,
    LanguageChanged,

    // Settings Menu
    SettingsMenuTitle,
    SettingsOptLanguage,
    SettingsOptApiCredentials,
    LanguageMenuTitle,
    SelectLanguageOption,

    // Profile Picker (startup)
    ProfilePickerTitle,
    ProfilePickerEntry,
    ProfilePickerAddNew,
    ProfilePickerChoicePrompt,
    ProfilePickerInvalid,
    FirstRunWelcome,
    PhoneNumberPrompt,

    // API Credentials Settings
    ApiCredentialsMenuTitle,
    ApiCredentialsCurrentEmbedded,
    ApiCredentialsCurrentCustom,
    ApiCredentialsOptUseOwn,
    ApiCredentialsOptGetOwn,
    ApiCredentialsOptRevertToEmbedded,
    ApiCredentialsChoicePrompt,
    ApiCredentialsPromptApiId,
    ApiCredentialsPromptApiHash,
    ApiCredentialsSaved,
    ApiCredentialsReverted,
    ApiCredentialsInstructions,
    ApiCredentialsEmbeddedUnavailable,

    // Manage Profiles Settings
    ManageProfilesMenuTitle,
    ManageProfilesEntry,
    ManageProfilesNoProfiles,
    ManageProfilesOptRemove,
    ManageProfilesOptSwitch,
    ManageProfilesChoicePrompt,
    ManageProfilesRemoveChoicePrompt,
    ManageProfilesRemoveConfirm,
    ManageProfilesRemoved,
    ManageProfilesSwitchConfirm,

    // Active Chat Operations
    FetchingActiveChats,
    SearchPrompt,
    NoMatchesFound,
    MatchingResultsHeader,
    EnterIndexPrompt,
    InvalidSelectionAlert,
    ScanningMetadata,
    FoundMessagesCount,
    SelectLevelTitle,
    Level1Desc,
    Level2Desc,
    Level3Desc,
    Level4Desc,
    Level0Desc,
    ChooseLevelPrompt,
    InvalidLevelSelected,
    LaunchingWipe,
    OperationComplete,

    // Surgical Depth Menu
    SurgicalDepthMenuTitle,
    SurgicalDepthOpt1,
    SurgicalDepthOpt2,
    SurgicalDepthOpt3,
    SurgicalDepthOpt4,
    SurgicalDepthOpt5,
    SurgicalDepthOpt0,
    SurgicalDepthChoicePrompt,
    CustomDepthPrompt,
    InvalidDepthInput,

    // Blocklist Manager
    BlocklistMenuTitle,
    BlocklistOpt1,
    BlocklistOpt2,
    BlocklistOpt3,
    BlocklistOpt0,
    BlocklistChoicePrompt,
    SyncingBlocklist,
    BlocklistCleanAlready,
    UnblockBotWarning,
    BatchLimitReached,
    RateLimitTriggered,
    ResumingPurge,
    SkippingToAvoidBan,
    EntityError,
    EntitiesProcessedSuccess,

    // Blocklist Operational Engine Logs
    WaitCooldownAlert,
    ForensicPurgeLog,

    // Useless Contacts Operational Engine Logs
    FindingContacts,
    MarkedForPurge,
    NoContactsFound,
    ConfirmContactPurge,
    PurgeProgress,
    ContactPurgeSuccess,

    // Active Chat Execution Traces & Success Metrics
    LogExecutingSurgical,
    LogExecutingForensic,
    LogDeepCleaningLinked,
    LogScanningLinkedGroup,
    LogTruncatedMainChannel,
    LogTruncatedLinkedGroup,
    LogDeletedLinkedSuccess,
    LogForensicWipeSuccess,

    // Active Chat Errors, Notes & Swallowed RpcExceptions
    LogNoLinkedMessagesFound,
    LogLinkedScanParticipantSkipped,
    LogLinkedCleanupBypassed,
    LogLinkedLeaveParticipantSkipped,
    LogMainLeaveSkipped,
    LogFootprintCleanupSkipped,
    LogCleanSlateError,
    LogDeletingPersonalMessages,
    LogTelegramApiError,
    LogUnlinkError,

    // Confirmations
    ConfirmLevel2,
    ConfirmLevel3,
    ConfirmLevel4,
    ConfirmBlocklistPurge,

    // Cancellation
    OperationCancelled,

    // Engine Boot/Shutdown Lifecycle Hooks
    EngineInitializing,
    LoginSuccess,
    EngineShutdown,

    // Shared Metadata Strings & Dynamic UI Format Fallbacks
    LabelChoicePrompt,
    LabelTitleHeader,
    FallbackUnknown,
    PrefixBot,
    PrefixUser,
    PrefixChatChannel,
}

public static class LocaleManager
{
    public static Language CurrentLanguage { get; set; } = Language.EN;

    private static readonly Dictionary<Language, Dictionary<TextKey, string>> Translations = new();

    static LocaleManager()
    {
        InitializeEnglishProfile();
        InitializeUzbekProfile();
        InitializeRussianProfile();
    }

    public static string T(TextKey key)
    {
        if (Translations.TryGetValue(CurrentLanguage, out var registry) && registry.TryGetValue(key, out var text))
        {
            return text;
        }

        return Translations.TryGetValue(Language.EN, out var defaultRegistry) && defaultRegistry.TryGetValue(key, out var defaultText)
            ? defaultText
            : $"[{key}]";
    }

    public static string T(TextKey key, params object[] args)
    {
        string pattern = T(key);
        try
        {
            return string.Format(pattern, args);
        }
        catch (FormatException)
        {
            return $"{pattern} (Parameters mismatch: {string.Join(", ", args)})";
        }
    }

    private static void InitializeEnglishProfile()
    {
        Translations[Language.EN] = new Dictionary<TextKey, string>
        {
            [TextKey.MainMenuTitle] = "--- NoTrace Engine Main Menu ---",
            [TextKey.MenuOption1] = "[1] Active Chat Operations (Search & Wipe)",
            [TextKey.MenuOption2] = "[2] Metadata: Blocklist Purge (Remove Blacklist Trail)",
            [TextKey.MenuOption3] = "[3] Metadata: Useless Contacts Purge (Clean Ghost Contacts)",
            [TextKey.MenuOption4] = "[4] Profile Management",
            [TextKey.MenuOption5] = "[5] Settings",
            [TextKey.MenuOption0] = "[0] Exit Engine",
            [TextKey.MenuOptBackToMain] = "[0] Back to Main Menu",
            [TextKey.MenuOptBack] = "[0] Back",
            [TextKey.SelectAction] = "Select action: ",
            [TextKey.InvalidSelection] = "Invalid selection. Try again.",
            [TextKey.LanguageChanged] = "Language successfully changed to English!",

            [TextKey.SettingsMenuTitle] = "--- Settings Menu ---",
            [TextKey.SettingsOptLanguage] = "[1] Language",
            [TextKey.SettingsOptApiCredentials] = "[2] API Credentials",
            [TextKey.LanguageMenuTitle] = "\n--- Language ---",
            [TextKey.SelectLanguageOption] = "Select Language / Tilni tanlang / Выберите язык:\n[1] O‘zbekcha\n[2] English\n[3] Русский\n[0] Back",

            [TextKey.ProfilePickerTitle] = "\n--- Select Account ---",
            [TextKey.ProfilePickerEntry] = "[{0}] {1} ({2})",
            [TextKey.ProfilePickerAddNew] = "[{0}] Add new account",
            [TextKey.ProfilePickerChoicePrompt] = "\nSelect account: ",
            [TextKey.ProfilePickerInvalid] = "Invalid selection. Try again.",
            [TextKey.FirstRunWelcome] = "\n--- Welcome to NoTrace ---\nNo saved accounts found. Let's log in for the first time.",
            [TextKey.PhoneNumberPrompt] = "Enter your phone number (with country code, e.g. +1234567890): ",

            [TextKey.ApiCredentialsMenuTitle] = "\n--- API Credentials ---",
            [TextKey.ApiCredentialsCurrentEmbedded] = "Currently using: Embedded (built-in, shared)",
            [TextKey.ApiCredentialsCurrentCustom] = "Currently using: Custom (your own API_ID/API_HASH)",
            [TextKey.ApiCredentialsOptUseOwn] = "[1] Switch to my own API_ID/API_HASH",
            [TextKey.ApiCredentialsOptGetOwn] = "[2] Get my own credentials",
            [TextKey.ApiCredentialsOptRevertToEmbedded] = "[3] Revert to embedded/default credentials",
            [TextKey.ApiCredentialsChoicePrompt] = "\nChoice: ",
            [TextKey.ApiCredentialsPromptApiId] = "Enter your API_ID: ",
            [TextKey.ApiCredentialsPromptApiHash] = "Enter your API_HASH: ",
            [TextKey.ApiCredentialsSaved] = "[SUCCESS] Custom API credentials saved. They'll be used from your next login.",
            [TextKey.ApiCredentialsReverted] = "[SUCCESS] Reverted to embedded/default credentials.",
            [TextKey.ApiCredentialsInstructions] = "\nTo get your own API_ID and API_HASH:\n 1. Go to https://my.telegram.org\n 2. Log in with your phone number\n 3. Open 'API development tools'\n 4. Create an app (any name/platform works)\n 5. Copy the API_ID and API_HASH shown there",
            [TextKey.ApiCredentialsEmbeddedUnavailable] = "[Note] This build has no embedded credentials. You must provide your own API_ID/API_HASH (via .env or this menu) to log in.",

            [TextKey.ManageProfilesMenuTitle] = "\n--- Manage Profiles ---",
            [TextKey.ManageProfilesEntry] = "[{0}] {1} ({2})",
            [TextKey.ManageProfilesNoProfiles] = "No saved profiles yet.",
            [TextKey.ManageProfilesOptRemove] = "[R] Remove a profile",
            [TextKey.ManageProfilesOptSwitch] = "[S] Switch active profile / add new account",
            [TextKey.ManageProfilesChoicePrompt] = "\nChoice: ",
            [TextKey.ManageProfilesRemoveChoicePrompt] = "\nSelect profile to remove: ",
            [TextKey.ManageProfilesRemoveConfirm] = "\n[!] This will delete the saved profile and its local session. You'll need to log in again to use it. Proceed? (y/n): ",
            [TextKey.ManageProfilesRemoved] = "[SUCCESS] Profile removed.",
            [TextKey.ManageProfilesSwitchConfirm] = "\n[!] This will return you to the account picker. Proceed? (y/n): ",

            [TextKey.FetchingActiveChats] = "\nFetching active conversation indices...",
            [TextKey.SearchPrompt] = "Search for a chat/bot name (type '0' to go back to Main Menu): ",
            [TextKey.NoMatchesFound] = "No matches found.",
            [TextKey.MatchingResultsHeader] = "\nFound {0} matching results:\n------------------------------------------",
            [TextKey.EnterIndexPrompt] = "\nEnter the [Index] to clean (or type '0' to search again): ",
            [TextKey.InvalidSelectionAlert] = "Invalid selection.",
            [TextKey.ScanningMetadata] = "Scanning footprint metadata...",
            [TextKey.FoundMessagesCount] = "Found {0} modern messages authored by you in local cache window.",
            [TextKey.SelectLevelTitle] = "\n--- Select NoTrace Level ---",
            [TextKey.Level1Desc] = "1. [Surgical]    - Delete ONLY my messages for everyone",
            [TextKey.Level2Desc] = "2. [Clean Slate] - Wipe all history (Stay in chat/list)",
            [TextKey.Level3Desc] = "3. [Unlink]      - Wipe history and REMOVE/LEAVE",
            [TextKey.Level4Desc] = "4. [Blacklist]   - Wipe history, REMOVE, and BLOCK",
            [TextKey.Level0Desc] = "0. [Cancel]      - Back to search",
            [TextKey.ChooseLevelPrompt] = "\nChoose Level (1-4 or 0): ",
            [TextKey.InvalidLevelSelected] = "Operation cancelled: Invalid level selected.",
            [TextKey.LaunchingWipe] = "\nLaunching Level {0} wipe operations...",
            [TextKey.OperationComplete] = "[SUCCESS] Operation complete.",

            [TextKey.SurgicalDepthMenuTitle] = "\n--- Surgical Depth Selection ---",
            [TextKey.SurgicalDepthOpt1] = "[1] Last 50 messages",
            [TextKey.SurgicalDepthOpt2] = "[2] Last 100 messages",
            [TextKey.SurgicalDepthOpt3] = "[3] Last 200 messages",
            [TextKey.SurgicalDepthOpt4] = "[4] Custom amount",
            [TextKey.SurgicalDepthOpt5] = "[5] All messages",
            [TextKey.SurgicalDepthOpt0] = "[0] Back",
            [TextKey.SurgicalDepthChoicePrompt] = "Select depth limit: ",
            [TextKey.CustomDepthPrompt] = "Enter custom message limit (numeric): ",
            [TextKey.InvalidDepthInput] = "[Error] Invalid number provided. Defaulting to 'All'.",

            [TextKey.BlocklistMenuTitle] = "\n--- Metadata: Blocklist Manager ---",
            [TextKey.BlocklistOpt1] = "[1] Unblock ONLY Bots (Clears automated metadata)",
            [TextKey.BlocklistOpt2] = "[2] Unblock ONLY Users (Clears human 1:1 trail)",
            [TextKey.BlocklistOpt3] = "[3] Unblock EVERYTHING (Total Forensic Purge)",
            [TextKey.BlocklistOpt0] = "[0] Back to Main Menu",
            [TextKey.BlocklistChoicePrompt] = "\nChoice: ",
            [TextKey.SyncingBlocklist] = "Fetching and syncing server-side blocklist...",
            [TextKey.BlocklistCleanAlready] = "Your blocklist is already clean.",
            [TextKey.UnblockBotWarning] = "[WARNING] Unblocking bot '{0}' will allow it to send you messages again.",
            [TextKey.BatchLimitReached] = "\n[BATCH LIMIT] 50 items reached. Cooldown for 10s...",
            [TextKey.RateLimitTriggered] = "\n[RATE LIMIT] Telegram anti-spam triggered.",
            [TextKey.ResumingPurge] = "\n[RESUMING] Retrying purge for {0}...\n",
            [TextKey.SkippingToAvoidBan] = "[ERROR] Skipping {0} to avoid permanent API ban.",
            [TextKey.EntityError] = "[ERROR] {0} failed: {1}",
            [TextKey.EntitiesProcessedSuccess] = "\n[SUCCESS] {0} entities processed.",

            [TextKey.WaitCooldownAlert] = "\r[WAIT] Sleeping for {0} seconds... Do not close the app.    ",
            [TextKey.ForensicPurgeLog] = "[{0}] [Forensic Purge] {1}",

            [TextKey.FindingContacts] = "Finding contacts with NO active chat history...",
            [TextKey.MarkedForPurge] = "[Marked for Purge] {0} {1} (@{2})",
            [TextKey.NoContactsFound] = "No useless ghost contacts found in address space metadata profiles.",
            [TextKey.ConfirmContactPurge] = "\nConfirm deletion of {0} contacts? (y/n): ",
            [TextKey.PurgeProgress] = "[Progress] Deleted {0}/{1}...",
            [TextKey.ContactPurgeSuccess] = "\n[SUCCESS] Contact list metadata minimized. Ghost contact references cleared.",

            [TextKey.LogExecutingSurgical] = "[NoTrace] Executing Surgical Footprint Purge...",
            [TextKey.LogExecutingForensic] = "[NoTrace] Executing Forensic Wipe (Mutual Erasure + Contact Purge)...",
            [TextKey.LogDeepCleaningLinked] = "Deep-cleaning linked group: {0}...",
            [TextKey.LogScanningLinkedGroup] = "[Surgical] Scanning linked discussion group: {0}...",
            [TextKey.LogTruncatedMainChannel] = "[Scoped] Truncated main channel target index to last {0} active units.",
            [TextKey.LogTruncatedLinkedGroup] = "[Scoped] Truncated linked group target index to last {0} active units.",
            [TextKey.LogDeletedLinkedSuccess] = "[SUCCESS] Deleted {0} messages from linked group '{1}'.",
            [TextKey.LogForensicWipeSuccess] = "[SUCCESS] Forensic Wipe: History and Contact link destroyed.",

            [TextKey.LogNoLinkedMessagesFound] = "[Note] No personal messages found in linked group '{0}'.",
            [TextKey.LogLinkedScanParticipantSkipped] = "[Note] Linked group surgical scan skipped: You are not a participant.",
            [TextKey.LogLinkedCleanupBypassed] = "[Minor] Linked group surgical cleanup bypassed: {0}",
            [TextKey.LogLinkedLeaveParticipantSkipped] = "[Note] Linked group leave skipped: You are not a participant of '{0}'.",
            [TextKey.LogMainLeaveSkipped] = "[Note] Main channel leave skipped: You are already not a participant.",
            [TextKey.LogFootprintCleanupSkipped] = "[Minor] Footprint cleanup skipped: {0}",
            [TextKey.LogCleanSlateError] = "[Error] Clean Slate failed: {0}",
            [TextKey.LogDeletingPersonalMessages] = "[Progress] Deleting your personal messages in group...",
            [TextKey.LogTelegramApiError] = "[Error] Telegram API error: {0}",
            [TextKey.LogUnlinkError] = "[Error] Unlink operation failed: {0}",

            [TextKey.EngineInitializing] = "--- NoTrace Engine Initializing ---",
            [TextKey.LoginSuccess] = "\nSuccess! Logged in as: {0} (ID: {1})",
            [TextKey.EngineShutdown] = "Exiting engine. Goodbye.",

            [TextKey.OperationCancelled] = "\n[Cancelled] Operation aborted. Returning to previous menu...",

            [TextKey.ConfirmLevel2] = "\n[!] This will wipe all chat history. Proceed? (y/n): ",
            [TextKey.ConfirmLevel3] = "\n[!] This will wipe history and leave. Proceed? (y/n): ",
            [TextKey.ConfirmLevel4] = "\n[!!] NUCLEAR OPTION — This will wipe all traces, BLOCK, and remove the contact permanently. Proceed? (y/n): ",
            [TextKey.ConfirmBlocklistPurge] = "\n[!] This will unblock and delete history for all matching entries. Unblocked bots/users will be able to message you again without your consent. Proceed? (y/n): ",

            [TextKey.LabelChoicePrompt] = "\nChoice: ",
            [TextKey.LabelTitleHeader] = " | Title: ",
            [TextKey.FallbackUnknown] = "Unknown",
            [TextKey.PrefixBot] = "[BOT] ",
            [TextKey.PrefixUser] = "[USER] ",
            [TextKey.PrefixChatChannel] = "[CHAT/CHANNEL] ",
        };
    }

    private static void InitializeUzbekProfile()
    {
        Translations[Language.UZ] = new Dictionary<TextKey, string>
        {
            [TextKey.MainMenuTitle] = "--- NoTrace Engine Asosiy Menyusi ---",
            [TextKey.MenuOption1] = "[1] Faol Suhbat Amallari (Qidiruv va Tozalash)",
            [TextKey.MenuOption2] = "[2] Metama'lumotlar: Bloklanganlar Ro‘yxatini Tozalash",
            [TextKey.MenuOption3] = "[3] Metama'lumotlar: Keraksiz Kontaktlarni Tozalash",
            [TextKey.MenuOption4] = "[4] Profillarni Boshqarish",
            [TextKey.MenuOption5] = "[5] Sozlamalar",
            [TextKey.MenuOption0] = "[0] Tizimdan Chiqish",
            [TextKey.MenuOptBackToMain] = "[0] Asosiy menyuga qaytish",
            [TextKey.MenuOptBack] = "[0] Orqaga",
            [TextKey.SelectAction] = "Harakatni tanlang: ",
            [TextKey.InvalidSelection] = "Noto‘g‘ri tanlov. Qaytadan urinib ko‘ring.",
            [TextKey.LanguageChanged] = "Til O‘zbek tiliga muvaffaqiyatli o‘zgartirildi!",

            [TextKey.SettingsMenuTitle] = "--- Sozlamalar Menyusi ---",
            [TextKey.SettingsOptLanguage] = "[1] Til",
            [TextKey.SettingsOptApiCredentials] = "[2] API Ma'lumotlari",
            [TextKey.LanguageMenuTitle] = "\n--- Til ---",
            [TextKey.SelectLanguageOption] = "Tilni tanlang / Select Language / Выберите язык:\n[1] O‘zbekcha\n[2] English\n[3] Русский\n[0] Orqaga",

            [TextKey.ProfilePickerTitle] = "\n--- Hisobni Tanlang ---",
            [TextKey.ProfilePickerEntry] = "[{0}] {1} ({2})",
            [TextKey.ProfilePickerAddNew] = "[{0}] Yangi hisob qo‘shish",
            [TextKey.ProfilePickerChoicePrompt] = "\nHisobni tanlang: ",
            [TextKey.ProfilePickerInvalid] = "Noto‘g‘ri tanlov. Qaytadan urinib ko‘ring.",
            [TextKey.FirstRunWelcome] = "\n--- NoTrace-ga xush kelibsiz ---\nSaqlangan hisoblar topilmadi. Birinchi marta tizimga kiramiz.",
            [TextKey.PhoneNumberPrompt] = "Telefon raqamingizni kiriting (davlat kodi bilan, masalan +998901234567): ",

            [TextKey.ApiCredentialsMenuTitle] = "\n--- API Ma'lumotlari ---",
            [TextKey.ApiCredentialsCurrentEmbedded] = "Hozir ishlatilmoqda: O‘rnatilgan (standart, umumiy)",
            [TextKey.ApiCredentialsCurrentCustom] = "Hozir ishlatilmoqda: Shaxsiy (sizning API_ID/API_HASH)",
            [TextKey.ApiCredentialsOptUseOwn] = "[1] O‘zimning API_ID/API_HASH'imga o‘tish",
            [TextKey.ApiCredentialsOptGetOwn] = "[2] O‘zimning ma'lumotlarimni olish",
            [TextKey.ApiCredentialsOptRevertToEmbedded] = "[3] O‘rnatilgan/standart ma'lumotlarga qaytish",
            [TextKey.ApiCredentialsChoicePrompt] = "\nTanlov: ",
            [TextKey.ApiCredentialsPromptApiId] = "API_ID kiriting: ",
            [TextKey.ApiCredentialsPromptApiHash] = "API_HASH kiriting: ",
            [TextKey.ApiCredentialsSaved] = "[MUVAFFAQIYAT] Shaxsiy API ma'lumotlari saqlandi. Keyingi tizimga kirishda ishlatiladi.",
            [TextKey.ApiCredentialsReverted] = "[MUVAFFAQIYAT] O‘rnatilgan/standart ma'lumotlarga qaytarildi.",
            [TextKey.ApiCredentialsInstructions] = "\nO‘zingizning API_ID va API_HASH olish uchun:\n 1. https://my.telegram.org saytiga o‘ting\n 2. Telefon raqamingiz bilan tizimga kiring\n 3. 'API development tools' bo‘limini oching\n 4. Ilova yarating (nomi/platformasi muhim emas)\n 5. Ko‘rsatilgan API_ID va API_HASH'ni nusxa oling",
            [TextKey.ApiCredentialsEmbeddedUnavailable] = "[Eslatma] Ushbu versiyada o‘rnatilgan ma'lumotlar mavjud emas. Tizimga kirish uchun o‘z API_ID/API_HASH'ingizni kiriting (.env orqali yoki shu menyuda).",

            [TextKey.ManageProfilesMenuTitle] = "\n--- Profillarni Boshqarish ---",
            [TextKey.ManageProfilesEntry] = "[{0}] {1} ({2})",
            [TextKey.ManageProfilesNoProfiles] = "Hozircha saqlangan profillar yo‘q.",
            [TextKey.ManageProfilesOptRemove] = "[R] Profilni o‘chirish",
            [TextKey.ManageProfilesOptSwitch] = "[S] Faol profilni almashtirish / yangi hisob qo‘shish",
            [TextKey.ManageProfilesChoicePrompt] = "\nTanlov: ",
            [TextKey.ManageProfilesRemoveChoicePrompt] = "\nO‘chiriladigan profilni tanlang: ",
            [TextKey.ManageProfilesRemoveConfirm] = "\n[!] Bu saqlangan profil va uning mahalliy sessiyasini o‘chiradi. Undan foydalanish uchun qayta tizimga kirishingiz kerak bo‘ladi. Davom etasizmi? (y/n): ",
            [TextKey.ManageProfilesRemoved] = "[MUVAFFAQIYAT] Profil o‘chirildi.",
            [TextKey.ManageProfilesSwitchConfirm] = "\n[!] Bu sizni hisob tanlash menyusiga qaytaradi. Davom etasizmi? (y/n): ",

            [TextKey.FetchingActiveChats] = "\nFaol suhbatlar indekslari yuklanmoqda...",
            [TextKey.SearchPrompt] = "Suhbat yoki bot nomini kiriting (Asosiy menyuga qaytish uchun '0'): ",
            [TextKey.NoMatchesFound] = "Hech qanday moslik topilmadi.",
            [TextKey.MatchingResultsHeader] = "\n{0} ta mos keladigan natija topildi:\n------------------------------------------",
            [TextKey.EnterIndexPrompt] = "\nTozalanadigan [Indeks]ni kiriting (yoki qayta qidirish uchun '0'): ",
            [TextKey.InvalidSelectionAlert] = "Noto‘g‘ri tanlov.",
            [TextKey.ScanningMetadata] = "Raqamli izlar tahlil qilinmoqda...",
            [TextKey.FoundMessagesCount] = "Kesh oynasida siz tomondan yozilgan {0} ta xabar topildi.",
            [TextKey.SelectLevelTitle] = "\n--- NoTrace Darajasini Tanlang ---",
            [TextKey.Level1Desc] = "1. [Zargarona]       - FAQAT mening xabarlarimni hamma uchun o‘chirish",
            [TextKey.Level2Desc] = "2. [Oq varaq]        - Barcha suhbat tarixini tozalash (guruhda/chatda qolish)",
            [TextKey.Level3Desc] = "3. [Aloqani uzish]   - Tarixni tozalash va O‘CHIRISH/CHIQISH",
            [TextKey.Level4Desc] = "4. [Qora ro‘yxat]    - Tarixni tozalash, aloqani mutloq uzish va BLOKLASH",
            [TextKey.Level0Desc] = "0. [Ortga]           - Qidiruvga qaytish",
            [TextKey.ChooseLevelPrompt] = "\nDarajani tanlang (1-4 yoki 0): ",
            [TextKey.InvalidLevelSelected] = "Amal bekor qilindi: Noto‘g‘ri daraja tanlandi.",
            [TextKey.LaunchingWipe] = "\n{0}-darajali tozalash amallari ishga tushirilmoqda...",
            [TextKey.OperationComplete] = "[MUVAFFAQIYAT] Amal bajarildi.",

            [TextKey.SurgicalDepthMenuTitle] = "\n--- Zargarona tozalash chuqurligi ---",
            [TextKey.SurgicalDepthOpt1] = "[1] Oxirgi 50 ta xabar",
            [TextKey.SurgicalDepthOpt2] = "[2] Oxirgi 100 ta xabar",
            [TextKey.SurgicalDepthOpt3] = "[3] Oxirgi 200 ta xabar",
            [TextKey.SurgicalDepthOpt4] = "[4] Maxsus miqdor (Qo‘lda kiritish)",
            [TextKey.SurgicalDepthOpt5] = "[5] Barcha xabarlar (Hammasi)",
            [TextKey.SurgicalDepthOpt0] = "[0] Orqaga",
            [TextKey.SurgicalDepthChoicePrompt] = "Tozalash chuqurligini tanlang: ",
            [TextKey.CustomDepthPrompt] = "Maxsus xabarlar sonini kiriting (raqam): ",
            [TextKey.InvalidDepthInput] = "[Xatolik] Noto‘g‘ri raqam kiritildi. 'Barchasi' rejimi tanlandi.",

            [TextKey.BlocklistMenuTitle] = "\n--- Metama'lumot: Bloklanganlar Nazorati ---",
            [TextKey.BlocklistOpt1] = "[1] FAQAT Botlarni blokdan chiqarish (Tizimli izlarni o‘chirish)",
            [TextKey.BlocklistOpt2] = "[2] FAQAT Foydalanuvchilarni chiqarish (Shaxsiy izlarni o‘chirish)",
            [TextKey.BlocklistOpt3] = "[3] HAMMASINI blokdan chiqarish (Mutlaq raqamli tozalash)",
            [TextKey.BlocklistOpt0] = "[0] Asosiy menyuga qaytish",
            [TextKey.BlocklistChoicePrompt] = "\nTanlov: ",
            [TextKey.SyncingBlocklist] = "Serverdagi bloklanganlar ro‘yxati sinxronizatsiya qilinmoqda...",
            [TextKey.BlocklistCleanAlready] = "Sizning qora ro‘yxatingiz allaqachon toza.",
            [TextKey.UnblockBotWarning] = "[OGOHLANTIRISH] '{0}' botini blokdan chiqarish unga sizga qayta xabar yuborish imkonini beradi.",
            [TextKey.BatchLimitReached] = "\n[PAKET CHEKLOVI] 50 ta element bajarildi. 10 soniya kutish...",
            [TextKey.RateLimitTriggered] = "\n[CHEKLOV]        Telegram anti-spam himoyasi faollashdi.",
            [TextKey.ResumingPurge] = "\n[TIKLANISH] {0} uchun tozalash qayta urinilmoqda...\n",
            [TextKey.SkippingToAvoidBan] = "[XATO] Doimiy bloklanishni oldini olish uchun {0} tashlab ketildi.",
            [TextKey.EntityError] = "[XATO] {0} bajarilmadi: {1}",
            [TextKey.EntitiesProcessedSuccess] = "\n[MUVAFFAQIYAT] {0} ta obyekt qayta ishlandi.",

            [TextKey.WaitCooldownAlert] = "\r[KUTISH] {0} soniya qoldi... Ilovani yopmang.    ",
            [TextKey.ForensicPurgeLog] = "[{0}] [Butkul tozalash] {1}",

            [TextKey.FindingContacts] = "Faol chatlar tarixiga ega bo‘lmagan 'arvoh' kontaktlar qidirilmoqda...",
            [TextKey.MarkedForPurge] = "[Tozalashga Belgilandi] {0} {1} (@{2})",
            [TextKey.NoContactsFound] = "Kontaktlar metama'lumotlarida faol bo‘lmagan 'arvoh' kontaktlar topilmadi.",
            [TextKey.ConfirmContactPurge] = "\nUshbu {0} ta kontaktni o‘chirishni tasdiqlaysizmi? (y/n): ",
            [TextKey.PurgeProgress] = "[Jarayon] O‘chirildi: {0}/{1}...",
            [TextKey.ContactPurgeSuccess] = "\n[MUVAFFAQIYAT] Kontaktlar ro‘yxati optimallashtirildi va 'arvoh' bog‘lanishlar tozalandi.",

            [TextKey.LogExecutingSurgical] = "[Izsiz] Zargarona izlarni tozalash operatsiyasi bajarilmoqda...",
            [TextKey.LogExecutingForensic] = "[Izsiz] Ekspertiza darajasidagi tozalash bajarilmoqda (O‘zaro o‘chirish + Kontaktlar tozalashi)...",
            [TextKey.LogDeepCleaningLinked] = "Bog‘langan guruh chuqur tozalanmoqda: {0}...",
            [TextKey.LogScanningLinkedGroup] = "[Zargarona] Bog‘langan muhokama guruhi skanerlanmoqda: {0}...",
            [TextKey.LogTruncatedMainChannel] = "[Ko‘lam] Asosiy kanal nishon ko‘lami oxirgi {0} ta xabargacha qisqartirildi.",
            [TextKey.LogTruncatedLinkedGroup] = "[Ko‘lam] Bog‘langan guruh nishon ko‘lami oxirgi {0} ta xabargacha qisqartirildi.",
            [TextKey.LogDeletedLinkedSuccess] = "[MUVAFFAQIYAT] '{1}' bog‘langan guruhidan {0} ta xabar muvaffaqiyatli o‘chirildi.",
            [TextKey.LogForensicWipeSuccess] = "[MUVAFFAQIYAT] Ekspertiza darajasidagi tozalash: Chat tarixi va kontakt tizimi aloqalari yo‘q qilindi.",

            [TextKey.LogNoLinkedMessagesFound] = "[Eslatma] '{0}' bog‘langan guruhida shaxsiy xabarlar topilmadi.",
            [TextKey.LogLinkedScanParticipantSkipped] = "[Eslatma] Bog‘langan guruh jarayoni o‘tkazib yuborildi: Siz ushbu guruh a'zosi emassiz.",
            [TextKey.LogLinkedCleanupBypassed] = "[Kichik Nosozlik] Bog‘langan guruh tozalash jarayoni chetlab o‘tildi: {0}",
            [TextKey.LogLinkedLeaveParticipantSkipped] = "[Eslatma] Guruhni tark etish amalga oshirilmadi: Siz '{0}' guruhining a'zosi emassiz.",
            [TextKey.LogMainLeaveSkipped] = "[Eslatma] Asosiy kanalni tark etish amalga oshirilmadi: Siz allaqachon a'zo emassiz.",
            [TextKey.LogFootprintCleanupSkipped] = "[Kichik Nosozlik] Izlarni o‘chirish amalga oshirilmadi: {0}",
            [TextKey.LogCleanSlateError] = "[Xatolik] Oq varaq bajarilmadi: {0}",
            [TextKey.LogDeletingPersonalMessages] = "[Jarayon] Guruhdagi shaxsiy xabarlaringiz o‘chirilmoqda...",
            [TextKey.LogTelegramApiError] = "[Xatolik] Telegram API xatosi: {0}",
            [TextKey.LogUnlinkError] = "[Xatolik] Aloqani uzishda muammo: {0}",

            [TextKey.EngineInitializing] = "--- NoTrace Dvigateli Ishga Tushmoqda ---",
            [TextKey.LoginSuccess] = "\nMuvaffaqiyatli ulanish! Tizimga kirildi: {0} (ID: {1})",
            [TextKey.EngineShutdown] = "Dvigateldan chiqilmoqda. Xayr.",

            [TextKey.OperationCancelled] = "\n[Bekor qilindi] Amal to'xtatildi. Oldingi menyuga qaytilmoqda...",

            [TextKey.ConfirmLevel2] = "\n[!] Bu barcha suhbat tarixini o'chiradi. Davom etasizmi? (y/n): ",
            [TextKey.ConfirmLevel3] = "\n[!] Bu tarixni o'chirib, chiqadi. Davom etasizmi? (y/n): ",
            [TextKey.ConfirmLevel4] = "\n[!!] YADROVIY VARIANT — Barcha izlar o'chiriladi, BLOKLANADI va kontakt butunlay yo'q qilinadi. Davom etasizmi? (y/n): ",
            [TextKey.ConfirmBlocklistPurge] = "\n[!] Bu mos yozuvlarning hammasini blokdan chiqarib, tarixini o'chiradi. Blokdan chiqarilgan botlar/foydalanuvchilar sizning roziligingizsiz yana xabar yubora oladi. Davom etasizmi? (y/n): ",

            [TextKey.LabelChoicePrompt] = "\nTanlov: ",
            [TextKey.LabelTitleHeader] = " | Nomi: ",
            [TextKey.PrefixBot] = "[BOT] ",
            [TextKey.PrefixUser] = "[FOYDALANUVCHI] ",
            [TextKey.PrefixChatChannel] = "[CHAT/KANAL] ",
        };
    }

    private static void InitializeRussianProfile()
    {
        Translations[Language.RU] = new Dictionary<TextKey, string>
        {
            [TextKey.MainMenuTitle] = "--- Главное Меню NoTrace Engine ---",
            [TextKey.MenuOption1] = "[1] Операции с активными чатами (Поиск и Очистка)",
            [TextKey.MenuOption2] = "[2] Метаданные: Очистка черного списка (Удалить след)",
            [TextKey.MenuOption3] = "[3] Метаданные: Очистка неактивных контактов",
            [TextKey.MenuOption4] = "[4] Управление Профилями",
            [TextKey.MenuOption5] = "[5] Настройки",
            [TextKey.MenuOption0] = "[0] Выйти из движка",
            [TextKey.MenuOptBackToMain] = "[0] Вернуться в главное меню",
            [TextKey.MenuOptBack] = "[0] Назад",
            [TextKey.SelectAction] = "Выберите действие: ",
            [TextKey.InvalidSelection] = "Неверный выбор. Попробуйте еще раз.",
            [TextKey.LanguageChanged] = "Язык успешно изменен на Русский!",

            [TextKey.SettingsMenuTitle] = "--- Меню Настроек ---",
            [TextKey.SettingsOptLanguage] = "[1] Язык",
            [TextKey.SettingsOptApiCredentials] = "[2] API Ключи",
            [TextKey.LanguageMenuTitle] = "\n--- Язык ---",
            [TextKey.SelectLanguageOption] = "Выберите язык / Tilni tanlang / Select Language:\n[1] O‘zbekcha\n[2] English\n[3] Русский\n[0] Назад",

            [TextKey.ProfilePickerTitle] = "\n--- Выберите Аккаунт ---",
            [TextKey.ProfilePickerEntry] = "[{0}] {1} ({2})",
            [TextKey.ProfilePickerAddNew] = "[{0}] Добавить новый аккаунт",
            [TextKey.ProfilePickerChoicePrompt] = "\nВыберите аккаунт: ",
            [TextKey.ProfilePickerInvalid] = "Неверный выбор. Попробуйте снова.",
            [TextKey.FirstRunWelcome] = "\n--- Добро пожаловать в NoTrace ---\nСохранённых аккаунтов не найдено. Войдём в систему впервые.",
            [TextKey.PhoneNumberPrompt] = "Введите номер телефона (с кодом страны, например +79991234567): ",

            [TextKey.ApiCredentialsMenuTitle] = "\n--- API Ключи ---",
            [TextKey.ApiCredentialsCurrentEmbedded] = "Сейчас используется: Встроенные (общие)",
            [TextKey.ApiCredentialsCurrentCustom] = "Сейчас используется: Свои (ваш API_ID/API_HASH)",
            [TextKey.ApiCredentialsOptUseOwn] = "[1] Перейти на свои API_ID/API_HASH",
            [TextKey.ApiCredentialsOptGetOwn] = "[2] Получить свои ключи",
            [TextKey.ApiCredentialsOptRevertToEmbedded] = "[3] Вернуться на встроенные/стандартные ключи",
            [TextKey.ApiCredentialsChoicePrompt] = "\nВыбор: ",
            [TextKey.ApiCredentialsPromptApiId] = "Введите API_ID: ",
            [TextKey.ApiCredentialsPromptApiHash] = "Введите API_HASH: ",
            [TextKey.ApiCredentialsSaved] = "[УСПЕХ] Свои API-ключи сохранены. Будут использованы при следующем входе.",
            [TextKey.ApiCredentialsReverted] = "[УСПЕХ] Возвращены встроенные/стандартные ключи.",
            [TextKey.ApiCredentialsInstructions] = "\nЧтобы получить свои API_ID и API_HASH:\n 1. Перейдите на https://my.telegram.org\n 2. Войдите с помощью своего номера телефона\n 3. Откройте 'API development tools'\n 4. Создайте приложение (название/платформа не важны)\n 5. Скопируйте показанные API_ID и API_HASH",
            [TextKey.ApiCredentialsEmbeddedUnavailable] = "[Заметка] В этой сборке нет встроенных ключей. Для входа укажите свои API_ID/API_HASH (через .env или это меню).",

            [TextKey.ManageProfilesMenuTitle] = "\n--- Управление Профилями ---",
            [TextKey.ManageProfilesEntry] = "[{0}] {1} ({2})",
            [TextKey.ManageProfilesNoProfiles] = "Сохранённых профилей пока нет.",
            [TextKey.ManageProfilesOptRemove] = "[R] Удалить профиль",
            [TextKey.ManageProfilesOptSwitch] = "[S] Сменить активный профиль / добавить новый аккаунт",
            [TextKey.ManageProfilesChoicePrompt] = "\nВыбор: ",
            [TextKey.ManageProfilesRemoveChoicePrompt] = "\nВыберите профиль для удаления: ",
            [TextKey.ManageProfilesRemoveConfirm] = "\n[!] Это удалит сохранённый профиль и его локальную сессию. Для повторного использования потребуется снова войти. Продолжить? (y/n): ",
            [TextKey.ManageProfilesRemoved] = "[УСПЕХ] Профиль удалён.",
            [TextKey.ManageProfilesSwitchConfirm] = "\n[!] Это вернёт вас к выбору аккаунта. Продолжить? (y/n): ",

            [TextKey.FetchingActiveChats] = "\nПолучение индексов активных диалогов...",
            [TextKey.SearchPrompt] = "Введите имя чата/бота (или '0' для возврата в Главное Меню): ",
            [TextKey.NoMatchesFound] = "Совпадений не найдено.",
            [TextKey.MatchingResultsHeader] = "\nНайдено {0} совпадающих результатов:\n------------------------------------------",
            [TextKey.EnterIndexPrompt] = "\nВведите [Индекс] для очистки (или '0' для повторного поиска): ",
            [TextKey.InvalidSelectionAlert] = "Неверный выбор.",
            [TextKey.ScanningMetadata] = "Сканирование метаданных цифрового следа...",
            [TextKey.FoundMessagesCount] = "В окне локального кэша найдено {0} ваших сообщений.",
            [TextKey.SelectLevelTitle] = "\n--- Выберите Уровень NoTrace ---",
            [TextKey.Level1Desc] = "1. [Ювелирный]      - Удалить ТОЛЬКО мои сообщения для всех",
            [TextKey.Level2Desc] = "2. [Чистый Лист]    - Стереть всю историю (Остаться в чате)",
            [TextKey.Level3Desc] = "3. [Отвязка]        - Стереть историю и УДАЛИТЬ/ВЫЙТИ",
            [TextKey.Level4Desc] = "4. [Чёрный список]  - Стереть историю, УДАЛИТЬ связь и ЗАБЛОКИРОВАТЬ",
            [TextKey.Level0Desc] = "0. [Назад]          - Вернуться к поиску",
            [TextKey.ChooseLevelPrompt] = "\nВыберите уровень (1-4 или 0): ",
            [TextKey.InvalidLevelSelected] = "Операция отменена: Выбран неверный уровень.",
            [TextKey.LaunchingWipe] = "\nЗапуск операции очистки уровня {0}...",
            [TextKey.OperationComplete] = "[УСПЕХ] Операция завершена.",

            [TextKey.SurgicalDepthMenuTitle] = "\n--- Глубина ювелирной очистки ---",
            [TextKey.SurgicalDepthOpt1] = "[1] Последние 50 сообщений",
            [TextKey.SurgicalDepthOpt2] = "[2] Последние 100 сообщений",
            [TextKey.SurgicalDepthOpt3] = "[3] Последние 200 сообщений",
            [TextKey.SurgicalDepthOpt4] = "[4] Указать свое количество",
            [TextKey.SurgicalDepthOpt5] = "[5] Все сообщения",
            [TextKey.SurgicalDepthOpt0] = "[0] Назад",
            [TextKey.SurgicalDepthChoicePrompt] = "Выберите лимит глубины: ",
            [TextKey.CustomDepthPrompt] = "Введите точное количество сообщений (число): ",
            [TextKey.InvalidDepthInput] = "[Ошибка] Введено неверное число. Выбран режим 'Все'.",

            [TextKey.BlocklistMenuTitle] = "\n--- Метаданные: Управление Чёрным Списком ---",
            [TextKey.BlocklistOpt1] = "[1] Разблокировать ТОЛЬКО ботов         (Удаление следов автоматизации)",
            [TextKey.BlocklistOpt2] = "[2] Разблокировать ТОЛЬКО пользователей (Удаление истории взаимодействия)",
            [TextKey.BlocklistOpt3] = "[3] Разблокировать ВСЁ                  (Тотальная зачистка метаданных)",
            [TextKey.BlocklistOpt0] = "[0] Вернуться в главное меню",
            [TextKey.BlocklistChoicePrompt] = "\nВаш выбор: ",
            [TextKey.SyncingBlocklist] = "Получение и синхронизация чёрного списка с сервером...",
            [TextKey.BlocklistCleanAlready] = "Ваш чёрный список уже пуст.",
            [TextKey.UnblockBotWarning] = "[ВНИМАНИЕ] Разблокировка бота '{0}' позволит ему снова отправлять вам сообщения.",
            [TextKey.BatchLimitReached] = "\n[ЛИМИТ ПАКЕТА] Обработано 50 элементов. Пауза 10 сек...",
            [TextKey.RateLimitTriggered] = "\n[ОГРАНИЧЕНИЕ]  Сработала антиспам-защита Telegram.",
            [TextKey.ResumingPurge] = "\n[ВОЗОБНОВЛЕНИЕ] Повторная попытка очистки для {0}...\n",
            [TextKey.SkippingToAvoidBan] = "[ОШИБКА] Пропуск {0} во избежание permanentной блокировки API.",
            [TextKey.EntityError] = "[ОШИБКА] Сбой {0}: {1}",
            [TextKey.EntitiesProcessedSuccess] = "\n[УСПЕХ] Обработано объектов: {0}.",

            [TextKey.WaitCooldownAlert] = "\r[ОЖИДАНИЕ] Тайм-аут {0} сек... Не закрывайте приложение.    ",
            [TextKey.ForensicPurgeLog] = "[{0}] [Судебная Очистка] {1}",

            [TextKey.FindingContacts] = "Поиск контактов без активной истории чатов и диалогов...",
            [TextKey.MarkedForPurge] = "[Намечен на удаление] {0} {1} (@{2})",
            [TextKey.NoContactsFound] = "В метаданных адресной книги не обнаружено неактивных призрачных контактов.",
            [TextKey.ConfirmContactPurge] = "\nПодтвердить удаление {0} контактов? (y/n): ",
            [TextKey.PurgeProgress] = "[Прогресс] Удалено {0}/{1}...",
            [TextKey.ContactPurgeSuccess] = "\n[УСПЕХ] Метаданные контактов минимизированы. Связи с призраками стерты.",

            [TextKey.LogExecutingSurgical] = "[Без Следа] Выполнение хирургической очистки цифрового следа...",
            [TextKey.LogExecutingForensic] = "[Без Следа] Запуск глубокого криминалистического стирания (Взаимное удаление + Контакты)...",
            [TextKey.LogDeepCleaningLinked] = "Глубокая зачистка связанной группы обсуждений: {0}...",
            [TextKey.LogScanningLinkedGroup] = "[Ювелирный] Сканирование привязанной группы комментариев: {0}...",
            [TextKey.LogTruncatedMainChannel] = "[Масштаб] Диапазон целей основного канала урезан до последних {0} единиц.",
            [TextKey.LogTruncatedLinkedGroup] = "[Масштаб] Диапазон целей связанной группы урезан до последних {0} единиц.",
            [TextKey.LogDeletedLinkedSuccess] = "[УСПЕХ] Удалено {0} сообщений из связанной группы '{1}'.",
            [TextKey.LogForensicWipeSuccess] = "[УСПЕХ] Судебная очистка: История переписки и связи аккаунта уничтожены.",

            [TextKey.LogNoLinkedMessagesFound] = "[Заметка] Личных сообщений в связанной группе '{0}' не найдено.",
            [TextKey.LogLinkedScanParticipantSkipped] = "[Заметка] Сканирование связанной группы пропущено: Вы не являетесь её участником.",
            [TextKey.LogLinkedCleanupBypassed] = "[Пропуск] Обход зачистки связанной группы комментариев: {0}",
            [TextKey.LogLinkedLeaveParticipantSkipped] = "[Заметка] Не удалось покинуть группу: Вы не являетесь участником '{0}'.",
            [TextKey.LogMainLeaveSkipped] = "[Заметка] Не удалось покинуть основной канал: Вы уже не состоите в нём.",
            [TextKey.LogFootprintCleanupSkipped] = "[Пропуск] Сбой удаления следов переписки: {0}",
            [TextKey.LogCleanSlateError] = "[Ошибка] Сбой очистки 'Чистый лист': {0}",
            [TextKey.LogDeletingPersonalMessages] = "[Прогресс] Удаление ваших личных сообщений в группе...",
            [TextKey.LogTelegramApiError] = "[Ошибка] Ошибка Telegram API: {0}",
            [TextKey.LogUnlinkError] = "[Ошибка] Сбой операции отвязки: {0}",

            [TextKey.EngineInitializing] = "--- Инициализация Движка NoTrace Engine ---",
            [TextKey.LoginSuccess] = "\nАвторизация успешна! Вход выполнен как: {0} (ID: {1})",
            [TextKey.EngineShutdown] = "Завершение работы движка. До свидания.",

            [TextKey.OperationCancelled] = "\n[Отменено] Операция прервана. Возврат в предыдущее меню...",

            [TextKey.ConfirmLevel2] = "\n[!] Это сотрёт всю историю чата. Продолжить? (y/n): ",
            [TextKey.ConfirmLevel3] = "\n[!] Это сотрёт историю и выйдет. Продолжить? (y/n): ",
            [TextKey.ConfirmLevel4] = "\n[!!] ЯДЕРНЫЙ ВАРИАНТ — Все следы будут уничтожены, контакт ЗАБЛОКИРОВАН и удалён навсегда. Продолжить? (y/n): ",
            [TextKey.ConfirmBlocklistPurge] = "\n[!] Это разблокирует все совпадающие записи и удалит их историю. Разблокированные боты/пользователи смогут снова писать вам без вашего согласия. Продолжить? (y/n): ",

            [TextKey.LabelChoicePrompt] = "\nВаш выбор: ",
            [TextKey.LabelTitleHeader] = " | Название: ",
            [TextKey.PrefixBot] = "[БОТ] ",
            [TextKey.PrefixUser] = "[ПОЛЬЗОВАТЕЛЬ] ",
            [TextKey.PrefixChatChannel] = "[ЧАТ/КАНАЛ] ",
        };
    }
}