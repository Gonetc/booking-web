using System.ComponentModel.DataAnnotations;

namespace BookingWeb.Models;

// ---------- API contracts used by the admin panel ----------

public record LoginRequestDto(string Username, string Password);

public record LoginResponseDto(string AccessToken, string TokenType, DateTime ExpiresAtUtc);

public record UpsertServiceDto(
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    bool IsActive);

public record UpsertMasterDto(
    string Name,
    string? Specialization,
    string? PhotoUrl,
    List<int> ServiceIds,
    List<WorkingHoursDto> WorkingHours,
    bool IsActive);

// ---------- Form models (what the admin edits on screen) ----------

public class LoginForm
{
    [Required(ErrorMessage = "Enter your username.")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Enter your password.")]
    public string Password { get; set; } = "";
}

public class ServiceForm
{
    [Required(ErrorMessage = "Enter a name.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = "";

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(5, 480, ErrorMessage = "Duration must be between 5 and 480 minutes.")]
    public int DurationMinutes { get; set; } = 60;

    [Range(0.0, 1_000_000.0, ErrorMessage = "Price cannot be negative.")]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public static ServiceForm From(ServiceDto s) => new()
    {
        Name = s.Name,
        Description = s.Description,
        DurationMinutes = s.DurationMinutes,
        Price = s.Price,
        IsActive = s.IsActive
    };

    public UpsertServiceDto ToDto() => new(
        Name.Trim(),
        string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
        DurationMinutes,
        Price,
        IsActive);
}

public class MasterForm
{
    /// <summary>Days in the order they are shown in the schedule editor.</summary>
    public static readonly DayOfWeek[] Week =
    [
        DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
        DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
    ];

    private string? _photoUrl;

    [Required(ErrorMessage = "Enter a name.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; } = "";

    [StringLength(100)]
    public string? Specialization { get; set; }

    [Url(ErrorMessage = "Enter a full URL starting with http:// or https://.")]
    [StringLength(500)]
    public string? PhotoUrl
    {
        get => _photoUrl;
        set => _photoUrl = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public bool IsActive { get; set; } = true;

    public HashSet<int> ServiceIds { get; set; } = [];

    public List<DayScheduleForm> Days { get; set; } =
        Week.Select(d => new DayScheduleForm { Day = d, Works = d is not (DayOfWeek.Saturday or DayOfWeek.Sunday) }).ToList();

    public static MasterForm From(MasterDto m) => new()
    {
        Name = m.Name,
        Specialization = m.Specialization,
        PhotoUrl = m.PhotoUrl,
        IsActive = m.IsActive,
        ServiceIds = m.ServiceIds.ToHashSet(),
        Days = Week.Select(day =>
        {
            var hours = m.WorkingHours.FirstOrDefault(h => h.DayOfWeek == day);
            return hours is null
                ? new DayScheduleForm { Day = day, Works = false }
                : new DayScheduleForm { Day = day, Works = true, Start = hours.StartTime, End = hours.EndTime };
        }).ToList()
    };

    /// <summary>Returns an error message if the schedule is inconsistent, otherwise null.</summary>
    public string? ValidateSchedule() =>
        Days.FirstOrDefault(d => d.Works && d.Start >= d.End) is { } bad
            ? $"{bad.Day}: the working day must start before it ends."
            : null;

    public UpsertMasterDto ToDto() => new(
        Name.Trim(),
        string.IsNullOrWhiteSpace(Specialization) ? null : Specialization.Trim(),
        PhotoUrl,
        ServiceIds.ToList(),
        Days.Where(d => d.Works).Select(d => new WorkingHoursDto(d.Day, d.Start, d.End)).ToList(),
        IsActive);
}

public class DayScheduleForm
{
    public DayOfWeek Day { get; set; }
    public bool Works { get; set; }
    public TimeOnly Start { get; set; } = new(10, 0);
    public TimeOnly End { get; set; } = new(19, 0);
}
