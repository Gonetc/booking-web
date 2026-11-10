using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using BookingWeb.Models;

namespace BookingWeb.Services;

/// <summary>JSON settings and error parsing shared by all API clients.</summary>
public static class ApiJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>Turns a ProblemDetails response into a human-readable message.</summary>
    public static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var fallback = $"Request failed ({(int)response.StatusCode}). Please try again.";
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDto>(Options, ct);
            if (problem?.Errors is { Count: > 0 } errors)
            {
                return string.Join(" ", errors.SelectMany(e => e.Value));
            }

            return problem?.Detail ?? problem?.Title ?? fallback;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            return fallback;
        }
    }
}
