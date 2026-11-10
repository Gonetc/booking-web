namespace BookingWeb.Models;

// Mirror of the Booking API contracts (see booking-api/src/BookingApi/Contracts)

public record ServiceDto(
    int Id,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    bool IsActive);

public record WorkingHoursDto(DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime);

public record MasterDto(
    int Id,
    string Name,
    string? Specialization,
    string? PhotoUrl,
    bool IsActive,
    List<int> ServiceIds,
    List<WorkingHoursDto> WorkingHours);

public record AvailabilityDto(int MasterId, int ServiceId, DateOnly Date, List<TimeOnly> Slots);

public record CreateBookingDto(
    int MasterId,
    int ServiceId,
    DateOnly Date,
    TimeOnly StartTime,
    string ClientName,
    string ClientPhone);

public record BookingDto(
    int Id,
    int MasterId,
    string MasterName,
    int ServiceId,
    string ServiceName,
    decimal Price,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string ClientName,
    string ClientPhone,
    string Status,
    DateTime CreatedAtUtc);

/// <summary>RFC 7807 error body returned by the API.</summary>
public record ProblemDto(
    string? Title,
    string? Detail,
    int? Status,
    Dictionary<string, string[]>? Errors);
