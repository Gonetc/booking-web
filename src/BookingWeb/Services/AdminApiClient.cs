using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookingWeb.Models;

namespace BookingWeb.Services;

/// <summary>
/// Typed HTTP client for the admin part of the Booking API.
/// Adds the admin's bearer token from <see cref="AdminSession"/> to every request.
/// </summary>
public class AdminApiClient(HttpClient http, AdminSession session)
{
    public Task<ApiResult<LoginResponseDto>> LoginAsync(string username, string password, CancellationToken ct = default) =>
        SendAsync<LoginResponseDto>(HttpMethod.Post, "api/auth/login", new LoginRequestDto(username, password), ct);

    // --- Bookings ---

    public Task<ApiResult<List<BookingDto>>> GetBookingsAsync(DateOnly date, CancellationToken ct = default) =>
        SendAsync<List<BookingDto>>(HttpMethod.Get, $"api/bookings?date={date:yyyy-MM-dd}", null, ct);

    public Task<ApiResult<BookingDto>> CancelBookingAsync(int id, CancellationToken ct = default) =>
        SendAsync<BookingDto>(HttpMethod.Post, $"api/bookings/{id}/cancel", null, ct);

    // --- Services ---

    public Task<ApiResult<List<ServiceDto>>> GetServicesAsync(CancellationToken ct = default) =>
        SendAsync<List<ServiceDto>>(HttpMethod.Get, "api/services?includeInactive=true", null, ct);

    public Task<ApiResult<ServiceDto>> CreateServiceAsync(UpsertServiceDto service, CancellationToken ct = default) =>
        SendAsync<ServiceDto>(HttpMethod.Post, "api/services", service, ct);

    public Task<ApiResult<ServiceDto>> UpdateServiceAsync(int id, UpsertServiceDto service, CancellationToken ct = default) =>
        SendAsync<ServiceDto>(HttpMethod.Put, $"api/services/{id}", service, ct);

    public Task<ApiResult<object>> DeactivateServiceAsync(int id, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Delete, $"api/services/{id}", null, ct);

    // --- Masters ---

    public Task<ApiResult<List<MasterDto>>> GetMastersAsync(CancellationToken ct = default) =>
        SendAsync<List<MasterDto>>(HttpMethod.Get, "api/masters", null, ct);

    public Task<ApiResult<MasterDto>> CreateMasterAsync(UpsertMasterDto master, CancellationToken ct = default) =>
        SendAsync<MasterDto>(HttpMethod.Post, "api/masters", master, ct);

    public Task<ApiResult<MasterDto>> UpdateMasterAsync(int id, UpsertMasterDto master, CancellationToken ct = default) =>
        SendAsync<MasterDto>(HttpMethod.Put, $"api/masters/{id}", master, ct);

    public Task<ApiResult<object>> DeactivateMasterAsync(int id, CancellationToken ct = default) =>
        SendAsync<object>(HttpMethod.Delete, $"api/masters/{id}", null, ct);

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, object? body, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, body.GetType(), options: ApiJson.Options);
        }

        if (session.AccessToken is { } token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        using var response = await http.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            return ApiResult<T>.Fail(await ApiJson.ReadErrorAsync(response, ct), (int)response.StatusCode);
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return ApiResult<T>.Ok(default!, (int)response.StatusCode);
        }

        var value = await response.Content.ReadFromJsonAsync<T>(ApiJson.Options, ct);
        return ApiResult<T>.Ok(value!, (int)response.StatusCode);
    }
}
