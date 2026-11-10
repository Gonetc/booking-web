using System.Security.Cryptography;
using BookingWeb.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BookingWeb.Services;

/// <summary>
/// Holds the admin's API token for the current browser tab.
/// The token is kept in sessionStorage, encrypted with ASP.NET Core Data Protection,
/// so it survives page reloads but not closing the tab, and cannot be read or forged by scripts.
/// Registered as scoped = one instance per Blazor circuit (per tab).
/// </summary>
public class AdminSession(ProtectedSessionStorage storage)
{
    private const string StorageKey = "admin-token";

    private StoredToken? _token;
    private bool _loaded;

    public bool IsAuthenticated => _token is not null && _token.ExpiresAtUtc > DateTime.UtcNow;

    public string? AccessToken => IsAuthenticated ? _token!.AccessToken : null;

    /// <summary>Reads the token from the browser. Requires JS interop, so call it after the first render.</summary>
    public async Task LoadAsync()
    {
        if (_loaded)
        {
            return;
        }

        try
        {
            var result = await storage.GetAsync<StoredToken>(StorageKey);
            _token = result.Success ? result.Value : null;
        }
        catch (CryptographicException)
        {
            // Encryption keys changed (e.g. app restarted in a container): treat as signed out
            await storage.DeleteAsync(StorageKey);
            _token = null;
        }

        _loaded = true;
    }

    public async Task SignInAsync(LoginResponseDto login)
    {
        _token = new StoredToken(login.AccessToken, login.ExpiresAtUtc);
        _loaded = true;
        await storage.SetAsync(StorageKey, _token);
    }

    public async Task SignOutAsync()
    {
        _token = null;
        await storage.DeleteAsync(StorageKey);
    }

    public sealed record StoredToken(string AccessToken, DateTime ExpiresAtUtc);
}
