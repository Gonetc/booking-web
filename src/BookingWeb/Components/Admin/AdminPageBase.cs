using BookingWeb.Services;
using Microsoft.AspNetCore.Components;

namespace BookingWeb.Components.Admin;

/// <summary>
/// Base class for admin pages: shared services, status messages
/// and uniform handling of API errors and expired sessions.
/// </summary>
public abstract class AdminPageBase : ComponentBase
{
    [Inject] protected AdminApiClient Api { get; set; } = null!;
    [Inject] protected AdminSession Session { get; set; } = null!;
    [Inject] protected NavigationManager Navigation { get; set; } = null!;

    protected string? Error { get; set; }
    protected string? Success { get; set; }

    /// <summary>
    /// Runs an API call. Returns the result on success; otherwise shows the error
    /// (or redirects to the login page if the session expired) and returns null.
    /// </summary>
    protected async Task<ApiResult<T>?> CallAsync<T>(Func<Task<ApiResult<T>>> call)
    {
        try
        {
            var result = await call();
            if (result.IsSuccess)
            {
                return result;
            }

            if (result.StatusCode == 401)
            {
                await Session.SignOutAsync();
                Navigation.NavigateTo("admin/login?expired=true");
                return null;
            }

            Error = result.Error;
            return null;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            Error = "The Booking API is unavailable. Please try again later.";
            return null;
        }
    }

    protected void ClearMessages()
    {
        Error = null;
        Success = null;
    }
}
