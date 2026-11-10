namespace BookingWeb.Services;

/// <summary>Public salon details shown on the site (configured in appsettings.json).</summary>
public class SalonInfo
{
    public const string SectionName = "Salon";

    public string Name { get; set; } = "Barbershop";
    public string Tagline { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Currency { get; set; } = "$";

    /// <summary>How many days ahead clients can book.</summary>
    public int BookingDaysAhead { get; set; } = 14;
}
