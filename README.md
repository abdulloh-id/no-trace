# NoTrace Engine

A fully open-source, localized console engine for managing and cleaning up your own Telegram account — bulk message wiping, blocklist management, and contact cleanup, all running entirely on your own machine.

## Architectural Security Philosophy

NoTrace is built with a zero-trust local execution design. Your credentials, session state tokens, and cryptographic keys are parsed exclusively in your local machine's RAM and disk (never dispatched to any external server other than Telegram's own). The entire codebase — orchestration layers, authentication gateways, and local caching mechanisms — is open for anyone to audit.

## Open Source

NoTrace was previously planned as an open-core project with a separate closed-source "Pro" tier. That split has been retired — the entire engine, including the deep-cleaning cleanup logic, is now fully open-source in this single repository, under the MIT License (see [`LICENSE`](./LICENSE)).

## Features

* **Active Chat Operations** — search any chat or bot and run cleanup at one of four levels, from deleting a limited number of your own recent messages up to a full "NoTrace" wipe (delete history, block, and remove the contact).
* **Blocklist Manager** — bulk-unblock and purge entries, with an explicit warning before unblocking a bot or user, since doing so lets them message you again.
* **Useless Contacts Purge** — clean up saved contacts you've never actually messaged.
* **Profile Management** — save multiple Telegram accounts and switch between them without relaunching the app.
* **Trilingual Interface** — fully localized in English, Oʻzbekcha (Uzbek), and Русский (Russian), switchable at runtime.
* **Flexible API Credentials** — use the shared credentials bundled with official release builds, or switch to your own `API_ID`/`API_HASH` at any time via Settings → API Credentials.

## ⚠️ Use At Your Own Risk

NoTrace performs **irreversible, destructive actions** on your Telegram account — including permanently deleting message history (for both sides of a conversation, where applicable), blocking contacts, and removing saved contacts. Please read this before using it:

* This software is provided **"AS IS", without warranty of any kind**, express or implied — see the [`LICENSE`](./LICENSE) file for the full disclaimer. The authors and contributors are not liable for any data loss, account restrictions, or other consequences arising from the use of this software.
* You are solely responsible for ensuring your use of this tool complies with **Telegram's Terms of Service**. Automated bulk actions (mass deletion, mass blocking/unblocking) may be flagged by Telegram's abuse-detection systems, and could result in temporary restrictions or other account-level consequences.
* Deletions performed at Level 2 and above use Telegram's "delete for everyone" mechanism where applicable — this is **not undoable**, by you or by Telegram.
* Always double-check your target chat and cleanup level before confirming a destructive action. Confirmation prompts exist for a reason — read them.
* If you use the shared, built-in API credentials from an official release build, be aware they are shared across everyone using that same release. Heavy or abusive use by any user of that credential pool could affect its reputation with Telegram for everyone using it. If you'd prefer isolation, switch to your own credentials via Settings → API Credentials.

If any of this is a dealbreaker, don't use this tool — or use it only after testing on a low-stakes account first.

## Getting Started

### Option 1 — Download a prebuilt release (recommended for most users)

Grab the binary for your OS from the [Releases](../../releases) page — no build tools required. See the release notes for first-run instructions and platform-specific notes (e.g. Linux may need `libicu`, and unsigned Windows/macOS binaries may trigger a SmartScreen/Gatekeeper warning on first launch).

### Option 2 — Build from source

#### Prerequisites

* [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or higher
* Your own Telegram `API_ID` and `API_HASH`, obtained for free from [my.telegram.org](https://my.telegram.org) (log in with your phone number → "API development tools" → create an app). Self-built binaries don't include the shared credentials bundled with official releases, so you'll need your own.

#### Build steps

1. Clone the repository:
   ```bash
   git clone https://github.com/abdulloh-id/no-trace.git
   ```
2. Navigate into the project:
   ```bash
   cd no-trace/NoTrace.Engine
   ```
3. Provide your credentials. Copy `.env.example` to `.env` and fill in your `API_ID` and `API_HASH`:
   ```bash
   cp .env.example .env
   ```
   Alternatively, skip this and set your credentials later from inside the app via Settings → API Credentials.
4. Build and run:
   ```bash
   dotnet build
   dotnet run
   ```

## Where Your Data Is Stored

Saved profiles, app settings, and Telegram session files are stored locally, never in this repository or anywhere online:

* **Windows:** `%AppData%\NoTrace\`
* **Linux/macOS:** `~/.config/NoTrace/`

Your 2FA password, if you have one, is never written to disk — it's only used transiently in memory during login.

## License

Licensed under the [MIT License](./LICENSE).