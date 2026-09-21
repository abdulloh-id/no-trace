// Inside NoTrace.Pro.Engine/Modules/TurboCleanupService.cs
using System.Threading.Tasks;
using TL;
using NoTrace.Engine.Core; // UserCanceledException + ICleanupService live here

namespace NoTrace.Engine.Modules;

// Explicitly inherit from the public interface contract
public class TurboCleanupService : ICleanupService
{
    private readonly WTelegram.Client _client;

    public TurboCleanupService(WTelegram.Client client)
    {
        _client = client;
    }

    /// <summary>
    /// Reads a line from the console. If the user types "0", throws UserCanceledException.
    /// Used only for prompts inside this service that need [0] back support.
    /// </summary>
    private static string ReadOrCancel(string prompt = "")
    {
        if (!string.IsNullOrEmpty(prompt))
            Console.Write(prompt);

        string? input = Console.ReadLine()?.Trim();

        if (input == "0")
            throw new UserCanceledException();

        return input ?? "";
    }

    /// <summary>
    /// Writes a success message in green, then resets the console color.
    /// </summary>
    private static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Deletes only the user's personal messages from a given target chat thread.
    /// Automatically routes between standard chats and supergroups/channels.
    /// </summary>
    public async Task DeleteMyFootprintAsync(InputPeer peer, int[] ids)
    {
        if (ids == null || ids.Length == 0) return;

        try
        {
            // Check if the target is a Channel or Supergroup (Linked Group)
            if (peer is InputPeerChannel inputChannel)
            {
                // Telegram requires a specific API endpoint for Supergroups/Channels
                await _client.Channels_DeleteMessages(inputChannel, ids);
            }
            else
            {
                // Standard endpoint for 1:1 Chats, Bots, and Legacy Basic Groups
                await _client.Messages_DeleteMessages(ids, revoke: true);
            }
        }
        catch (UserCanceledException) { throw; } // Always propagate cancel
        catch (Exception ex)
        {
            Console.WriteLine($"[Minor] Footprint cleanup skipped: {ex.Message}");
        }
    }

    /// <summary>
    /// Scans a target's history and returns message IDs authored by the current logged-in user.
    /// </summary>
    public async Task<int[]> ScanMyMessageIdsAsync(InputPeer inputTarget)
    {
        var history = await _client.Messages_GetHistory(inputTarget, limit: 100);
        if (history is Messages_MessagesBase messagesBase)
        {
            return messagesBase.Messages
                .OfType<Message>()
                .Where(m => m.from_id?.ID == _client.User.ID)
                .Select(m => m.id)
                .ToArray();
        }
        return Array.Empty<int>();
    }

    /// <summary>
    /// Executes Active Chat Operations based on levels 1 to 4.
    /// Throws UserCanceledException if the user enters [0] at any prompt.
    /// </summary>
    public async Task ExecuteChatWipeAsync(IPeerInfo target, string levelChoice, int[] myMessageIds, int? surgicalLimit = null)
    {
        var inputTarget = target.ToInputPeer();

        switch (levelChoice)
        {
            case "1":
                Console.WriteLine(LocaleManager.T(TextKey.LogExecutingSurgical));

                // 1. Clean up messages in the main chat/channel thread
                int[] targetedIds = myMessageIds;
                if (surgicalLimit.HasValue)
                {
                    targetedIds = myMessageIds.Take(surgicalLimit.Value).ToArray();
                    Console.WriteLine(LocaleManager.T(TextKey.LogTruncatedMainChannel, targetedIds.Length));
                }

                await DeleteMyFootprintAsync(inputTarget, targetedIds);

                // 2. Safely scan and purge the linked group discussion
                if (target is Channel mainChannel)
                {
                    try
                    {
                        var dialogs = await _client.Messages_GetAllDialogs();
                        var fullChannel = await _client.Channels_GetFullChannel(mainChannel);

                        if (fullChannel.full_chat is ChannelFull cf && cf.linked_chat_id != 0)
                        {
                            if (dialogs.chats.TryGetValue(cf.linked_chat_id, out var linkedChat) && linkedChat is Channel linkedChannelObj)
                            {
                                var linkedPeer = linkedChat.ToInputPeer();
                                Console.WriteLine(LocaleManager.T(TextKey.LogScanningLinkedGroup, linkedChat.Title));

                                int searchOffset = 0;
                                List<Message> linkedMessagesList = new List<Message>();

                                while (true)
                                {
                                    var searchResults = await _client.Messages_Search(linkedPeer, "", from_id: _client.User, offset_id: searchOffset, limit: 100);
                                    if (searchResults is not Messages_MessagesBase ms || ms.Messages.Length == 0) break;

                                    var messagesFound = ms.Messages.OfType<Message>().ToList();
                                    linkedMessagesList.AddRange(messagesFound);

                                    searchOffset = messagesFound.LastOrDefault()?.id ?? 0;
                                    if (searchOffset == 0 || ms.Messages.Length < 100) break;
                                }

                                int[] linkedTargetedIds = linkedMessagesList
                                    .OrderByDescending(m => m.id)
                                    .Select(m => m.id)
                                    .ToArray();

                                if (surgicalLimit.HasValue)
                                {
                                    linkedTargetedIds = linkedTargetedIds.Take(surgicalLimit.Value).ToArray();
                                    Console.WriteLine(LocaleManager.T(TextKey.LogTruncatedLinkedGroup, linkedTargetedIds.Length));
                                }

                                if (linkedTargetedIds.Length > 0)
                                {
                                    await _client.Channels_DeleteMessages(linkedChannelObj, linkedTargetedIds);
                                    WriteSuccess(LocaleManager.T(TextKey.LogDeletedLinkedSuccess, linkedTargetedIds.Length, linkedChat.Title));
                                }
                                else
                                {
                                    Console.WriteLine(LocaleManager.T(TextKey.LogNoLinkedMessagesFound, linkedChat.Title));
                                }
                            }
                        }
                    }
                    catch (UserCanceledException) { throw; }
                    catch (RpcException rpcEx) when (rpcEx.Code == 400 || rpcEx.Message.Contains("USER_NOT_PARTICIPANT"))
                    {
                        Console.WriteLine(LocaleManager.T(TextKey.LogLinkedScanParticipantSkipped));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(LocaleManager.T(TextKey.LogLinkedCleanupBypassed, ex.Message));
                    }
                }
                break;

            case "2":
                try
                {
                    Messages_AffectedHistory result;
                    do
                    {
                        result = await _client.Messages_DeleteHistory(inputTarget, max_id: int.MaxValue, just_clear: true, revoke: true);
                        if (result.offset > 0)
                            await Task.Delay(300);
                    } while (result.offset > 0);

                    Console.WriteLine(LocaleManager.T(TextKey.OperationComplete));
                }
                catch (UserCanceledException) { throw; }
                catch (Exception ex)
                {
                    Console.WriteLine(LocaleManager.T(TextKey.LogCleanSlateError, ex.Message));
                }
                break;

            case "3":
                try
                {
                    if (target is User u)
                    {
                        Messages_AffectedHistory result;
                        do
                        {
                            result = await _client.Messages_DeleteHistory(inputTarget, max_id: 0, just_clear: false, revoke: true);
                            if (result.offset > 0)
                                await Task.Delay(300);
                        } while (result.offset > 0);

                        try
                        {
                            await _client.Contacts_DeleteContacts(new InputUserBase[] { u });
                        }
                        catch { /* Ignore if user was not in contacts */ }

                        // Using existing TextKey.LogForensicWipeSuccess from Localization_2.cs
                        Console.WriteLine(LocaleManager.T(TextKey.LogForensicWipeSuccess));
                    }
                    else
                    {
                        // ... groups/channels branch unchanged ...
                    }
                }
                catch (UserCanceledException) { throw; }
                catch (RpcException rpcEx)
                {
                    Console.WriteLine(LocaleManager.T(TextKey.LogTelegramApiError, rpcEx.Message));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(LocaleManager.T(TextKey.LogUnlinkError, ex.Message));
                }
                break;

            case "4":
                if (target is Channel channel)
                {
                    var dialogs = await _client.Messages_GetAllDialogs();
                    var fullChannel = await _client.Channels_GetFullChannel(channel);
                    if (fullChannel.full_chat is ChannelFull cf && cf.linked_chat_id != 0)
                    {
                        if (dialogs.chats.TryGetValue(cf.linked_chat_id, out var linkedChat) && linkedChat is Channel linkedChannelObj)
                        {
                            var linkedPeer = linkedChat.ToInputPeer();
                            Console.WriteLine(LocaleManager.T(TextKey.LogDeepCleaningLinked, linkedChat.Title));

                            try
                            {
                                int searchOffset = 0;
                                while (true)
                                {
                                    var searchResults = await _client.Messages_Search(linkedPeer, "", from_id: _client.User, offset_id: searchOffset, limit: 100);
                                    if (searchResults is not Messages_MessagesBase ms || ms.Messages.Length == 0) break;

                                    var messagesFound = ms.Messages.OfType<Message>().ToList();
                                    var foundIds = messagesFound.Select(m => m.id).ToArray();
                                    if (foundIds.Length > 0)
                                    {
                                        await _client.Channels_DeleteMessages(linkedChannelObj, foundIds);
                                        WriteSuccess(LocaleManager.T(TextKey.LogDeletedLinkedSuccess, foundIds.Length, linkedChat.Title));
                                        await Task.Delay(2000);
                                    }
                                    searchOffset = messagesFound.LastOrDefault()?.id ?? 0;
                                    if (searchOffset == 0 || ms.Messages.Length < 100) break;
                                }

                                await _client.Channels_LeaveChannel(linkedChannelObj);
                            }
                            catch (UserCanceledException) { throw; }
                            catch (RpcException rpcEx) when (rpcEx.Code == 400 || rpcEx.Message.Contains("USER_NOT_PARTICIPANT"))
                            {
                                Console.WriteLine(LocaleManager.T(TextKey.LogLinkedLeaveParticipantSkipped, linkedChat.Title));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(LocaleManager.T(TextKey.LogLinkedCleanupBypassed, ex.Message));
                            }
                        }
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Logic for Option 2: Synchronizes and deep-cleans the server-side blocklist.
    /// Throws UserCanceledException if the user enters [0] at the block-type prompt.
    /// </summary>
    public async Task PurgeBlocklistAsync(string blockChoice)
    {
        // [0] cancel already handled by MenuController reading blockChoice,
        // but guard here in case it's called directly with "0"
        if (blockChoice == "0")
            throw new UserCanceledException();

        Console.WriteLine(LocaleManager.T(TextKey.SyncingBlocklist));
        var blocked = await _client.Contacts_GetBlocked();

        int count = 0;
        int batchCounter = 0;

        if (blocked.blocked.Length == 0)
        {
            Console.WriteLine(LocaleManager.T(TextKey.BlocklistCleanAlready));
            return;
        }

        foreach (var b in blocked.blocked)
        {
            if (!blocked.users.TryGetValue(b.peer_id.ID, out var userBase) || userBase is not User u)
                continue;

            bool shouldUnblock = false;
            if (blockChoice == "1" && u.IsBot) shouldUnblock = true;
            else if (blockChoice == "2" && !u.IsBot) shouldUnblock = true;
            else if (blockChoice == "3") shouldUnblock = true;

            if (shouldUnblock)
            {
                string name = u.username ?? u.first_name ?? "Unknown";

                // Warn before unblocking bots specifically, since that restores their ability to message again
                if (u.IsBot)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(LocaleManager.T(TextKey.UnblockBotWarning, name));
                    Console.ResetColor();
                }

                int retryCount = 0;

                batchCounter++;
                if (batchCounter > 50)
                {
                    Console.WriteLine(LocaleManager.T(TextKey.BatchLimitReached));
                    await Task.Delay(10000);
                    batchCounter = 1;
                }

                while (true)
                {
                    try
                    {
                        var inputPeer = u.ToInputPeer();
                        await _client.Messages_DeleteHistory(inputPeer, max_id: 0, just_clear: false, revoke: true);
                        await _client.Contacts_Unblock(inputPeer);
                        await Task.Delay(500);
                        await _client.Messages_DeleteHistory(inputPeer, max_id: 0, just_clear: false, revoke: true);

                        count++;
                        Console.WriteLine($"[{count}] [Forensic Purge] {name}");
                        await Task.Delay(1000);
                        break;
                    }
                    catch (RpcException ex) when (ex.Code == 420)
                    {
                        Console.WriteLine(LocaleManager.T(TextKey.RateLimitTriggered));
                        int waitTime = ex.X + 2;

                        for (int i = waitTime; i > 0; i--)
                        {
                            Console.Write($"\r[WAIT] Sleeping for {i} seconds... Do not close the app.   ");
                            await Task.Delay(1000);
                        }

                        Console.WriteLine(LocaleManager.T(TextKey.ResumingPurge, name));
                        retryCount++;

                        if (retryCount >= 3)
                        {
                            Console.WriteLine(LocaleManager.T(TextKey.SkippingToAvoidBan, name));
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(LocaleManager.T(TextKey.EntityError, name, ex.Message));
                        break;
                    }
                }
            }
        }
        WriteSuccess(LocaleManager.T(TextKey.EntitiesProcessedSuccess, count));
    }

    /// <summary>
    /// Logic for Option 3: Identifies and targets contacts with zero conversational footprint.
    /// Throws UserCanceledException if the user enters [0] at the confirmation prompt.
    /// </summary>
    public async Task PurgeUselessContactsAsync()
    {
        Console.WriteLine(LocaleManager.T(TextKey.FindingContacts));

        var contacts = await _client.Contacts_GetContacts();
        var dialogs = await _client.Messages_GetAllDialogs();
        var activeIds = dialogs.users.Keys.ToHashSet();

        var toDelete = new List<InputUserBase>();

        foreach (var u in contacts.users.Values.OfType<User>())
        {
            if (!activeIds.Contains(u.id))
            {
                toDelete.Add(u);
                Console.WriteLine(LocaleManager.T(TextKey.MarkedForPurge,
                    u.first_name ?? "",
                    u.last_name ?? "",
                    u.username ?? ""));
            }
        }

        if (toDelete.Count == 0)
        {
            Console.WriteLine(LocaleManager.T(TextKey.NoContactsFound));
            return;
        }

        // [0] Back support on the confirmation prompt
        Console.WriteLine("\n  [0] Back — cancel and return to menu");
        string confirm = ReadOrCancel(LocaleManager.T(TextKey.ConfirmContactPurge, toDelete.Count));

        if (confirm.ToLower() == "y")
        {
            int contactPurgeCount = 0;
            for (int i = 0; i < toDelete.Count; i += 20)
            {
                var batch = toDelete.Skip(i).Take(20).ToArray();
                await _client.Contacts_DeleteContacts(batch);
                contactPurgeCount += batch.Length;
                Console.WriteLine(LocaleManager.T(TextKey.PurgeProgress, contactPurgeCount, toDelete.Count));
                await Task.Delay(2000);
            }
            WriteSuccess(LocaleManager.T(TextKey.ContactPurgeSuccess));
        }
    }
}