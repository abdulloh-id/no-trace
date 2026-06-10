using System.Threading.Tasks;
using TL;

namespace NoTrace.Engine.Core;

/// <summary>
/// Thrown when the user enters [0] at any prompt to cancel back to the previous menu level.
/// Defined in the public core so both NoTrace.Engine.UI and NoTrace.Pro.Engine can reference it
/// without any circular dependency.
/// </summary>
public sealed class UserCanceledException : Exception
{
    public UserCanceledException() : base("Operation cancelled by user.") { }
}

/// <summary>
/// Defines architectural contracts for message-wiping, blocklist routines, and metadata cleanup.
/// </summary>
public interface ICleanupService
{
    Task<int[]> ScanMyMessageIdsAsync(InputPeer target);
    Task ExecuteChatWipeAsync(IPeerInfo target, string level, int[] messageIds, int? limit);
    Task PurgeBlocklistAsync(string option);
    Task PurgeUselessContactsAsync();
}
