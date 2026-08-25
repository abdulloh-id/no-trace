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
    MenuOption0,
    SelectAction,
    InvalidSelection,
    LanguageChanged,

    // Settings Menu
    SettingsMenuTitle,
    SelectLanguageOption,

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

    // GUI specific keys
    GuiLoginTitle,
    GuiSignInTabHeader,
    GuiAddAccountTabHeader,
    GuiPhoneInputLabel,
    GuiPhoneInputWatermark,
    GuiSendCodeButton,
    GuiCodeInputLabel,
    GuiVerifyButton,
    GuiTwoFaLabel,
    GuiLoadingConnecting,
    GuiWelcomeUser,
    GuiMainWindowTitle,
    GuiUpgradeToPremium,
    GuiProfileDropdownLabel,
    GuiSearchPlaceholder,
    GuiNoChatsFound,
    GuiChatsHeader,
    GuiSelectedChatLabel,
    GuiMessagesFoundLabel,
    GuiSurgicalLimitLabel,
    GuiSurgicalLimitTooltip,
    GuiLevelSelectionHeader,
    GuiLevel1Name,
    GuiLevel2Name,
    GuiLevel3Name,
    GuiLevel4Name,
    GuiPremiumLockTooltip,
    GuiBlocklistTitle,
    GuiBlocklistBots,
    GuiBlocklistUsers,
    GuiBlocklistAll,
    GuiContactsPurgeTitle,
    GuiScanContactsButton,
    GuiUpgradeBannerText,
    GuiUpgradeButtonText,
    GuiLicenseLabel,
    GuiLicenseWatermark,
    GuiActivateButton,
    GuiInvalidLicenseError,
    GuiConnectionError,
    GuiLicenseTiedError,
    GuiProfileListLabel,
    GuiAddProfileButton,
    GuiDeleteProfileButton,
    GuiActiveProfileBadge,
    GuiTabChats,
    GuiTabBlocklist,
    GuiTabContacts,
    GuiTabLicense,
    GuiConfirmTitle,
    GuiWipeConfirmationText,
    LogoSubText,
    GuiPhoneInstructionsText,
    DidNotReceiveText,
    ResendCodeLink,
    GuiExecuteWipeButton
}

/// <summary>
/// Provides high-performance, centralized localizations supporting Sentence-case UI design targets.
/// </summary>
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

        // Defensive Fallback Path to avoid blank UI strings or exceptions
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
            // Fail-safe protection layer preventing unhandled crashes on structural translation mismatches
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
            [TextKey.MenuOption4] = "[4] Settings",
            [TextKey.MenuOption0] = "[0] Exit Engine",
            [TextKey.SelectAction] = "Select action: ",
            [TextKey.InvalidSelection] = "Invalid selection. Try again.",
            [TextKey.LanguageChanged] = "Language successfully changed to English!",

            [TextKey.SettingsMenuTitle] = "--- Settings Menu ---",
            [TextKey.SelectLanguageOption] = "Select Language / Tilni tanlang / Выберите язык:\n[1] O‘zbekcha\n[2] English\n[3] Русский\n[0] Back",

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
            [TextKey.Level3Desc] = "3. [Blacklist]   - Wipe history and BLOCK/LEAVE",
            [TextKey.Level4Desc] = "4. [NoTrace]     - Wipe history and REMOVE (Zero metadata)",
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

            [TextKey.EngineInitializing] = "--- NoTrace Engine Initializing ---",
            [TextKey.LoginSuccess] = "\nSuccess! Logged in as: {0} (ID: {1})",
            [TextKey.EngineShutdown] = "Exiting engine. Goodbye.",

            [TextKey.OperationCancelled] = "\n[Cancelled] Operation aborted. Returning to previous menu...",

            [TextKey.ConfirmLevel2] = "\n[!] This will wipe all chat history. Proceed? (y/n): ",
            [TextKey.ConfirmLevel3] = "\n[!] This will wipe history and block/leave. Proceed? (y/n): ",
            [TextKey.ConfirmLevel4] = "\n[!!] NUCLEAR OPTION — This will wipe all traces and remove the contact permanently. Proceed? (y/n): ",
            [TextKey.ConfirmBlocklistPurge] = "\n[!] This will unblock and delete history for all matching entries. Proceed? (y/n): ",

            [TextKey.LabelChoicePrompt] = "\nChoice: ",
            [TextKey.LabelTitleHeader] = " | Title: ",
            [TextKey.FallbackUnknown] = "Unknown",
            [TextKey.PrefixBot] = "[BOT] ",
            [TextKey.PrefixUser] = "[USER] ",
            [TextKey.PrefixChatChannel] = "[CHAT/CHANNEL] ",

            // GUI English translations
            [TextKey.GuiLoginTitle] = "NoTrace Login",
            [TextKey.GuiSignInTabHeader] = "Sign in",
            [TextKey.GuiAddAccountTabHeader] = "Add account",
            [TextKey.GuiPhoneInputLabel] = "Phone number",
            [TextKey.GuiPhoneInputWatermark] = "e.g. +1 555 000 0000",
            [TextKey.GuiSendCodeButton] = "Continue",
            [TextKey.GuiCodeInputLabel] = "Enter the code from Telegram",
            [TextKey.GuiVerifyButton] = "Verify",
            [TextKey.GuiTwoFaLabel] = "2FA Password",
            [TextKey.GuiLoadingConnecting] = "Connecting to Telegram...",
            [TextKey.GuiWelcomeUser] = "Welcome, {0}!",
            [TextKey.GuiMainWindowTitle] = "NoTrace - Digital Footprint Manager",
            [TextKey.GuiUpgradeToPremium] = "Upgrade to Premium",
            [TextKey.GuiProfileDropdownLabel] = "Active Account",
            [TextKey.GuiSearchPlaceholder] = "Search active chats...",
            [TextKey.GuiNoChatsFound] = "No matching chats found.",
            [TextKey.GuiChatsHeader] = "Conversations",
            [TextKey.GuiSelectedChatLabel] = "Selected: {0}",
            [TextKey.GuiMessagesFoundLabel] = "Found {0} personal messages in local history.",
            [TextKey.GuiSurgicalLimitLabel] = "Message limit:",
            [TextKey.GuiSurgicalLimitTooltip] = "Limit surgical deletion to the last N messages (newest first).",
            [TextKey.GuiLevelSelectionHeader] = "Select NoTrace Level",
            [TextKey.GuiLevel1Name] = "Level 1: Surgical (Delete only my messages)",
            [TextKey.GuiLevel2Name] = "Level 2: Clean Slate (Wipe chat history)",
            [TextKey.GuiLevel3Name] = "Level 3: Blacklist (Wipe history & Block/Leave)",
            [TextKey.GuiLevel4Name] = "Level 4: Forensic (Zero Metadata & delete contact)",
            [TextKey.GuiPremiumLockTooltip] = "Premium feature. Upgrade to unlock this level.",
            [TextKey.GuiBlocklistTitle] = "Metadata: Blocklist Manager",
            [TextKey.GuiBlocklistBots] = "Unblock ONLY Bots",
            [TextKey.GuiBlocklistUsers] = "Unblock ONLY Users",
            [TextKey.GuiBlocklistAll] = "Unblock Everything (Total Forensic Purge)",
            [TextKey.GuiContactsPurgeTitle] = "Metadata: Ghost Contacts Purge",
            [TextKey.GuiScanContactsButton] = "Scan Address Book",
            [TextKey.GuiUpgradeBannerText] = "Unlock Multi-Profile, All Levels, and Premium Support",
            [TextKey.GuiUpgradeButtonText] = "Activate License",
            [TextKey.GuiLicenseLabel] = "Lemon Squeezy License Key",
            [TextKey.GuiLicenseWatermark] = "Paste your license key here...",
            [TextKey.GuiActivateButton] = "Activate Premium",
            [TextKey.GuiInvalidLicenseError] = "Invalid license key. Please check and try again.",
            [TextKey.GuiConnectionError] = "Could not connect to license validation server.",
            [TextKey.GuiLicenseTiedError] = "This license is already registered on another computer.",
            [TextKey.GuiProfileListLabel] = "Saved Accounts",
            [TextKey.GuiAddProfileButton] = "Add Account",
            [TextKey.GuiDeleteProfileButton] = "Remove",
            [TextKey.GuiActiveProfileBadge] = "Active",
            [TextKey.GuiTabChats] = "Active Chats",
            [TextKey.GuiTabBlocklist] = "Blocklist Purge",
            [TextKey.GuiTabContacts] = "Contacts Purge",
            [TextKey.GuiTabLicense] = "Licensing",
            [TextKey.GuiConfirmTitle] = "Confirm Action",
            [TextKey.GuiWipeConfirmationText] = "Are you sure you want to execute {0}? This action cannot be undone.",
            [TextKey.LogoSubText] = "DIGITAL FOOTPRINT MANAGER",
            [TextKey.GuiPhoneInstructionsText] = "A verification code will be sent to your Telegram app",
            [TextKey.DidNotReceiveText] = "Didn't receive it? ",
            [TextKey.ResendCodeLink] = "Resend code",
            [TextKey.GuiExecuteWipeButton] = "Execute Wipe"
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
            [TextKey.MenuOption4] = "[4] Sozlamalar",
            [TextKey.MenuOption0] = "[0] Tizimdan Chiqish",
            [TextKey.SelectAction] = "Harakatni tanlang: ",
            [TextKey.InvalidSelection] = "Noto‘g‘ri tanlov. Qaytadan urinib ko‘ring.",
            [TextKey.LanguageChanged] = "Til O‘zbek tiliga muvaffaqiyatli o‘zgartirildi!",

            [TextKey.SettingsMenuTitle] = "--- Sozlamalar Menyusi ---",
            [TextKey.SelectLanguageOption] = "Tilni tanlang / Select Language / Выберите язык:\n[1] O‘zbekcha\n[2] English\n[3] Русский\n[0] Orqaga",

            [TextKey.FetchingActiveChats] = "\nFaol suhbatlar indekslari yuklanmoqda...",
            [TextKey.SearchPrompt] = "Suhbat yoki bot nomini kiriting (Asosiy menyuga qaytish uchun '0'): ",
            [TextKey.NoMatchesFound] = "Hech qanday moslik topilmadi.",
            [TextKey.MatchingResultsHeader] = "\n{0} ta mos keladigan natija topildi:\n------------------------------------------",
            [TextKey.EnterIndexPrompt] = "\nTozalanadigan [Indeks]ni kiriting (yoki qayta qidirish uchun '0'): ",
            [TextKey.InvalidSelectionAlert] = "Noto‘g‘ri tanlov.",
            [TextKey.ScanningMetadata] = "Raqamli izlar tahlil qilinmoqda...",
            [TextKey.FoundMessagesCount] = "Kesh oynasida siz tomondan yozilgan {0} ta xabar topildi.",
            [TextKey.SelectLevelTitle] = "\n--- NoTrace Darajasini Tanlang ---",
            [TextKey.Level1Desc] = "1. [Zargarona]      - FAQAT mening xabarlarimni hamma uchun o‘chirish",
            [TextKey.Level2Desc] = "2. [Oq varaq]       - Barcha suhbat tarixini tozalash (guruhda qolish)",
            [TextKey.Level3Desc] = "3. [Qora ro‘yxat]    - Tarixni tozalash va BLOKLASH/CHIQISH",
            [TextKey.Level4Desc] = "4. [Izsiz]          - Tarixni o‘chirish va aloqani mutloq uzish (Metasiz)",
            [TextKey.Level0Desc] = "0. [Ortga]          - Qidiruvga qaytish",
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

            [TextKey.EngineInitializing] = "--- NoTrace Dvigateli Ishga Tushmoqda ---",
            [TextKey.LoginSuccess] = "\nMuvaffaqiyatli ulanish! Tizimga kirildi: {0} (ID: {1})",
            [TextKey.EngineShutdown] = "Dvigateldan chiqilmoqda. Xayr.",

            [TextKey.OperationCancelled] = "\n[Bekor qilindi] Amal to'xtatildi. Oldingi menyuga qaytilmoqda...",

            [TextKey.ConfirmLevel2] = "\n[!] Bu barcha suhbat tarixini o'chiradi. Davom etasizmi? (y/n): ",
            [TextKey.ConfirmLevel3] = "\n[!] Bu tarixni o'chirib, bloklaydi/chiqadi. Davom etasizmi? (y/n): ",
            [TextKey.ConfirmLevel4] = "\n[!!] YADROVIY VARIANT — Barcha izlar o'chiriladi va kontakt butunlay yo'q qilinadi. Davom etasizmi? (y/n): ",
            [TextKey.ConfirmBlocklistPurge] = "\n[!] Bu mos yozuvlarning hammasini blokdan chiqarib, tarixini o'chiradi. Davom etasizmi? (y/n): ",

            [TextKey.LabelChoicePrompt] = "\nTanlov: ",
            [TextKey.LabelTitleHeader] = " | Nomi: ",
            [TextKey.PrefixBot] = "[BOT] ",
            [TextKey.PrefixUser] = "[FOYDALANUVCHI] ",
            [TextKey.PrefixChatChannel] = "[CHAT/KANAL] ",

            // GUI Uzbek translations
            [TextKey.GuiLoginTitle] = "NoTrace Kirish",
            [TextKey.GuiSignInTabHeader] = "Tizimga kirish",
            [TextKey.GuiAddAccountTabHeader] = "Hisob qo‘shish",
            [TextKey.GuiPhoneInputLabel] = "Telefon raqami",
            [TextKey.GuiPhoneInputWatermark] = "Masalan: +998 90 123 4567",
            [TextKey.GuiSendCodeButton] = "Davom etish",
            [TextKey.GuiCodeInputLabel] = "Telegramdagi tasdiqlash kodini kiriting",
            [TextKey.GuiVerifyButton] = "Tasdiqlash",
            [TextKey.GuiTwoFaLabel] = "Ikki bosqichli parol (2FA)",
            [TextKey.GuiLoadingConnecting] = "Telegramga ulanmoqda...",
            [TextKey.GuiWelcomeUser] = "Xush kelibsiz, {0}!",
            [TextKey.GuiMainWindowTitle] = "NoTrace - Raqamli Izlarni Tozalash",
            [TextKey.GuiUpgradeToPremium] = "Premiumga o‘tish",
            [TextKey.GuiProfileDropdownLabel] = "Faol Hisob",
            [TextKey.GuiSearchPlaceholder] = "Suhbatlarni qidirish...",
            [TextKey.GuiNoChatsFound] = "Mos keladigan suhbatlar topilmadi.",
            [TextKey.GuiChatsHeader] = "Suhbatlar",
            [TextKey.GuiSelectedChatLabel] = "Tanlandi: {0}",
            [TextKey.GuiMessagesFoundLabel] = "Keshdan siz yozgan {0} ta xabar topildi.",
            [TextKey.GuiSurgicalLimitLabel] = "Xabarlar cheklovi:",
            [TextKey.GuiSurgicalLimitTooltip] = "Zargarona o‘chirishni oxirgi N ta xabar bilan cheklash (yangi xabarlardan boshlab).",
            [TextKey.GuiLevelSelectionHeader] = "NoTrace Darajasini Tanlang",
            [TextKey.GuiLevel1Name] = "1-daraja: Zargarona (Faqat shaxsiy xabarlarni o‘chirish)",
            [TextKey.GuiLevel2Name] = "2-daraja: Oq varaq (Barcha suhbat tarixini tozalash)",
            [TextKey.GuiLevel3Name] = "3-daraja: Qora ro‘yxat (Tarixni tozalash va bloklash/chiqish)",
            [TextKey.GuiLevel4Name] = "4-daraja: Izsiz (Aloqani mutloq uzish va ghost kontaktni o‘chirish)",
            [TextKey.GuiPremiumLockTooltip] = "Premium imkoniyat. Ushbu darajani ochish uchun Premium faollashtiring.",
            [TextKey.GuiBlocklistTitle] = "Metama'lumotlar: Qora Ro‘yxat Nazorati",
            [TextKey.GuiBlocklistBots] = "Faqat Botlarni blokdan chiqarish",
            [TextKey.GuiBlocklistUsers] = "Faqat Foydalanuvchilarni chiqarish",
            [TextKey.GuiBlocklistAll] = "Hammasini chiqarish (Mutlaq tozalash)",
            [TextKey.GuiContactsPurgeTitle] = "Metama'lumotlar: 'Arvoh' Kontaktlar Tozalashi",
            [TextKey.GuiScanContactsButton] = "Kontaktlarni Skanerlash",
            [TextKey.GuiUpgradeBannerText] = "Ko‘p profillilik, barcha darajalar va Premium qo‘llab-quvvatlashni faollashtiring",
            [TextKey.GuiUpgradeButtonText] = "Litsenziyani Faollashtirish",
            [TextKey.GuiLicenseLabel] = "Lemon Squeezy Litsenziya Kaliti",
            [TextKey.GuiLicenseWatermark] = "Litsenziya kalitini shu yerga joylashtiring...",
            [TextKey.GuiActivateButton] = "Premiumga o‘tish",
            [TextKey.GuiInvalidLicenseError] = "Noto‘g‘ri litsenziya kaliti. Tekshirib qayta urinib ko‘ring.",
            [TextKey.GuiConnectionError] = "Litsenziyani tasdiqlash serveriga ulanib bo‘lmadi.",
            [TextKey.GuiLicenseTiedError] = "Ushbu litsenziya boshqa kompyuterga bog‘langan.",
            [TextKey.GuiProfileListLabel] = "Saqlangan Hisoblar",
            [TextKey.GuiAddProfileButton] = "Hisob Qo‘shish",
            [TextKey.GuiDeleteProfileButton] = "O‘chirish",
            [TextKey.GuiActiveProfileBadge] = "Faol",
            [TextKey.GuiTabChats] = "Faol Chats",
            [TextKey.GuiTabBlocklist] = "Bloklanganlar",
            [TextKey.GuiTabContacts] = "Kontaktlarni Tozalash",
            [TextKey.GuiTabLicense] = "Litsenziyalash",
            [TextKey.GuiConfirmTitle] = "Amalni Tasdiqlash",
            [TextKey.GuiWipeConfirmationText] = "Haqiqatan ham {0} amalni bajarmoqchimisiz? Bu amalni bekor qilib bo‘lmaydi.",
            [TextKey.LogoSubText] = "RAQAMLI IZLARNI NAZORAT QILISH",
            [TextKey.GuiPhoneInstructionsText] = "Tasdiqlash kodi Telegram ilovangizga yuboriladi",
            [TextKey.DidNotReceiveText] = "Kod kelmadimi? ",
            [TextKey.ResendCodeLink] = "Kodni qayta yuborish",
            [TextKey.GuiExecuteWipeButton] = "Tozalashni bajarish"
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
            [TextKey.MenuOption4] = "[4] Настройки",
            [TextKey.MenuOption0] = "[0] Выйти из движка",
            [TextKey.SelectAction] = "Выберите действие: ",
            [TextKey.InvalidSelection] = "Неверный выбор. Попробуйте еще раз.",
            [TextKey.LanguageChanged] = "Язык успешно изменен на Русский!",

            [TextKey.SettingsMenuTitle] = "--- Меню Настроек ---",
            [TextKey.SelectLanguageOption] = "Выберите язык / Tilni tanlang / Select Language:\n[1] O‘zbekcha\n[2] English\n[3] Русский\n[0] Назад",

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
            [TextKey.Level3Desc] = "3. [Чёрный Список]  - Стереть историю и ЗАБЛОКИРОВАТЬ/ВЫЙТИ",
            [TextKey.Level4Desc] = "4. [Без Следа]      - Стереть историю и УДАЛИТЬ связь (Без метаданных)",
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

            [TextKey.EngineInitializing] = "--- Инициализация Движка NoTrace Engine ---",
            [TextKey.LoginSuccess] = "\nАвторизация успешна! Вход выполнен как: {0} (ID: {1})",
            [TextKey.EngineShutdown] = "Завершение работы движка. До свидания.",

            [TextKey.OperationCancelled] = "\n[Отменено] Операция прервана. Возврат в предыдущее меню...",

            [TextKey.ConfirmLevel2] = "\n[!] Это сотрёт всю историю чата. Продолжить? (y/n): ",
            [TextKey.ConfirmLevel3] = "\n[!] Это сотрёт историю и заблокирует/выйдет. Продолжить? (y/n): ",
            [TextKey.ConfirmLevel4] = "\n[!!] ЯДЕРНЫЙ ВАРИАНТ — Все следы будут уничтожены, контакт удалён навсегда. Продолжить? (y/n): ",
            [TextKey.ConfirmBlocklistPurge] = "\n[!] Это разблокирует все совпадающие записи и удалит их историю. Продолжить? (y/n): ",

            [TextKey.LabelChoicePrompt] = "\nВаш выбор: ",
            [TextKey.LabelTitleHeader] = " | Название: ",
            [TextKey.PrefixBot] = "[БОТ] ",
            [TextKey.PrefixUser] = "[ПОЛЬЗОВАТЕЛЬ] ",
            [TextKey.PrefixChatChannel] = "[ЧАТ/КАНАЛ] ",

            // GUI Russian translations
            [TextKey.GuiLoginTitle] = "Вход NoTrace",
            [TextKey.GuiSignInTabHeader] = "Войти",
            [TextKey.GuiAddAccountTabHeader] = "Добавить аккаунт",
            [TextKey.GuiPhoneInputLabel] = "Номер телефона",
            [TextKey.GuiPhoneInputWatermark] = "Например: +7 999 000 0000",
            [TextKey.GuiSendCodeButton] = "Продолжить",
            [TextKey.GuiCodeInputLabel] = "Введите код из Telegram",
            [TextKey.GuiVerifyButton] = "Подтвердить",
            [TextKey.GuiTwoFaLabel] = "Двухфакторный пароль (2FA)",
            [TextKey.GuiLoadingConnecting] = "Подключение к Telegram...",
            [TextKey.GuiWelcomeUser] = "Добро пожаловать, {0}!",
            [TextKey.GuiMainWindowTitle] = "NoTrace - Менеджер Цифрового Следа",
            [TextKey.GuiUpgradeToPremium] = "Перейти на Premium",
            [TextKey.GuiProfileDropdownLabel] = "Активный Аккаунт",
            [TextKey.GuiSearchPlaceholder] = "Поиск активных чатов...",
            [TextKey.GuiNoChatsFound] = "Совпадающих чатов не найдено.",
            [TextKey.GuiChatsHeader] = "Диалоги",
            [TextKey.GuiSelectedChatLabel] = "Выбрано: {0}",
            [TextKey.GuiMessagesFoundLabel] = "В кэше найдено {0} ваших сообщений.",
            [TextKey.GuiSurgicalLimitLabel] = "Лимит сообщений:",
            [TextKey.GuiSurgicalLimitTooltip] = "Ограничить хирургическое удаление последними N сообщениями (начиная с новых).",
            [TextKey.GuiLevelSelectionHeader] = "Выберите Уровень NoTrace",
            [TextKey.GuiLevel1Name] = "Уровень 1: Ювелирный (Удалить только мои сообщения)",
            [TextKey.GuiLevel2Name] = "Уровень 2: Чистый Лист (Очистить историю чата)",
            [TextKey.GuiLevel3Name] = "Уровень 3: Чёрный Список (Стереть историю и блок/выход)",
            [TextKey.GuiLevel4Name] = "Уровень 4: Без Следа (Удалить все метаданные и контакт)",
            [TextKey.GuiPremiumLockTooltip] = "Премиум-возможность. Активируйте Premium для разблокировки.",
            [TextKey.GuiBlocklistTitle] = "Метаданные: Управление Чёрным Списком",
            [TextKey.GuiBlocklistBots] = "Разблокировать ТОЛЬКО Ботов",
            [TextKey.GuiBlocklistUsers] = "Разблокировать ТОЛЬКО Пользователей",
            [TextKey.GuiBlocklistAll] = "Разблокировать Всё (Полная криминалистическая очистка)",
            [TextKey.GuiContactsPurgeTitle] = "Метаданные: Очистка Контактов-Призраков",
            [TextKey.GuiScanContactsButton] = "Сканировать Контакты",
            [TextKey.GuiUpgradeBannerText] = "Разблокируйте многопрофильность, все уровни очистки и Premium-поддержку",
            [TextKey.GuiUpgradeButtonText] = "Активировать Лицензию",
            [TextKey.GuiLicenseLabel] = "Лицензионный Ключ Lemon Squeezy",
            [TextKey.GuiLicenseWatermark] = "Вставьте ваш лицензионный ключ сюда...",
            [TextKey.GuiActivateButton] = "Активировать Premium",
            [TextKey.GuiInvalidLicenseError] = "Неверный лицензионный ключ. Проверьте и попробуйте еще раз.",
            [TextKey.GuiConnectionError] = "Не удалось подключиться к серверу проверки лицензий.",
            [TextKey.GuiLicenseTiedError] = "Эта лицензия уже привязана к другому компьютеру.",
            [TextKey.GuiProfileListLabel] = "Сохраненные Аккаунты",
            [TextKey.GuiAddProfileButton] = "Добавить Аккаунт",
            [TextKey.GuiDeleteProfileButton] = "Удалить",
            [TextKey.GuiActiveProfileBadge] = "Активен",
            [TextKey.GuiTabChats] = "Активные Чаты",
            [TextKey.GuiTabBlocklist] = "Черный Список",
            [TextKey.GuiTabContacts] = "Очистить Контакты",
            [TextKey.GuiTabLicense] = "Лицензирование",
            [TextKey.GuiConfirmTitle] = "Подтверждение Действия",
            [TextKey.GuiWipeConfirmationText] = "Вы уверены, что хотите выполнить {0}? Это действие необратимо.",
            [TextKey.LogoSubText] = "МЕНЕДЖЕР ЦИФРОВОГО СЛЕДА",
            [TextKey.GuiPhoneInstructionsText] = "Код подтверждения будет отправлен в ваше приложение Telegram",
            [TextKey.DidNotReceiveText] = "Не получили код? ",
            [TextKey.ResendCodeLink] = "Отправить код повторно",
            [TextKey.GuiExecuteWipeButton] = "Выполнить очистку"
        };
    }
}