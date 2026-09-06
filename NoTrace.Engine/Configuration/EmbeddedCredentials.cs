namespace NoTrace.Engine.Configuration;

/// <summary>
/// Optional API credentials baked into official NoTrace release builds only.
/// Left blank in the public source tree — self-build/clone users must supply
/// their own credentials via .env, and any user can override via the in-app
/// "API Credentials" settings menu.
/// </summary>
public static class EmbeddedCredentials
{
    public const string ApiId = "";
    public const string ApiHash = "";

    public static bool IsAvailable =>
        !string.IsNullOrWhiteSpace(ApiId) && !string.IsNullOrWhiteSpace(ApiHash);
}
