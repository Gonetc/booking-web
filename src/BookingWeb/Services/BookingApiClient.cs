using System.Net.Http.Json;
using BookingWeb.Models;

namespace BookingWeb.Services;

/// <summary>Typed HTTP client for the public part of the Booking API.</summary>
public class BookingApiClient(HttpClient http)
{
    public async Task<IReadOnlyList<ServiceDto>> GetServicesAsync(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<List<ServiceDto>>("api/services", ApiJson.Options, ct) ?? [];

    public async Task<IReadOnlyList<MasterDto>> GetMastersAsync(int? serviceId = null, CancellationToken ct = default)
    {
        var url = serviceId is null ? "api/masters" : $"api/masters?serviceId={serviceId}";
        return await http.GetFromJsonAsync<List<MasterDto>>(url, ApiJson.Options, ct) ?? [];
    }

    public async Task<IReadOnlyList<TimeOnly>> GetAvailabilityAsync(
        int masterId, int serviceId, DateOnly date, CancellationToken ct = default)
    {
        var url = $"api/masters/{masterId}/availability?serviceId={serviceId}&date={date:yyyy-MM-dd}";
        var result = await http.GetFromJsonAsync<AvailabilityDto>(url, ApiJson.Options, ct);
        return result?.Slots ?? [];
    }

    public async Task<ApiResult<BookingDto>> CreateBookingAsync(CreateBookingDto request, CancellationToken ct = default)
    {
        using var response = await http.PostAsJsonAsync("api/bookings", request, ApiJson.Options, ct);
        if (response.IsSuccessStatusCode)
        {
            var booking = await response.Content.ReadFromJsonAsync<BookingDto>(ApiJson.Options, ct);
            return ApiResult<BookingDto>.Ok(booking!, (int)response.StatusCode);
        }

        return ApiResult<BookingDto>.Fail(await ApiJson.ReadErrorAsync(response, ct), (int)response.StatusCode);
    }
}
