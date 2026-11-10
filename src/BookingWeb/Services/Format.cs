using System.Globalization;

namespace BookingWeb.Services;

/// <summary>Display formatting. The site is in English regardless of the server's regional settings.</summary>
public static class Format
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en-US");

    public static string Price(decimal price, string currency) => currency + price.ToString("0.##", Culture);

    public static string Duration(int minutes) => minutes switch
    {
        < 60 => $"{minutes} min",
        _ when minutes % 60 == 0 => $"{minutes / 60} h",
        _ => $"{minutes / 60} h {minutes % 60} min"
    };

    public static string Time(TimeOnly time) => time.ToString("HH:mm", Culture);

    public static string LongDate(DateOnly date) => date.ToString("dddd, MMMM d", Culture);

    public static string Weekday(DateOnly date) => date.ToString("ddd", Culture);

    public static string ShortDate(DateOnly date) => date.ToString("MMM d", Culture);

    public static string Initials(string name) =>
        string.Concat(name
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(part => char.ToUpperInvariant(part[0])));
}
