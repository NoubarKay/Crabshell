namespace Crabshell.Core.Attributes.Fields;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class DateTimeFieldAttribute : CrabshellFieldAttribute
{
    /// <summary>Include time component. Maps to timestamptz. False = date only.</summary>
    public bool HasTime { get; set; } = true;

    /// <summary>Show only time picker, no date calendar.</summary>
    public bool TimeOnly { get; set; } = false;

    /// <summary>Store as UTC.</summary>
    public bool Utc { get; set; } = true;

    /// <summary>Minimum allowed date. Format: "yyyy-MM-dd"</summary>
    public string? Min { get; set; }

    /// <summary>Maximum allowed date. Format: "yyyy-MM-dd"</summary>
    public string? Max { get; set; }

    /// <summary>Show a "set to now" button in the admin UI.</summary>
    public bool ShowNowButton { get; set; } = false;

    /// <summary>Hour step increment in the time picker. Default 1.</summary>
    public int HoursStep { get; set; } = 1;

    /// <summary>Minute step increment in the time picker. Default 1.</summary>
    public int MinutesStep { get; set; } = 1;

    /// <summary>Second step increment in the time picker. Default 1.</summary>
    public int SecondsStep { get; set; } = 1;

    /// <summary>Update value immediately as user types. Default false.</summary>
    public bool Immediate { get; set; } = false;

    /// <summary>Show the calendar inline (always visible). Default false.</summary>
    public bool Inline { get; set; } = false;

    /// <summary>Hide the calendar icon button. Default false.</summary>
    public bool ShowButton { get; set; } = true;

    /// <summary>Year range shown in the year dropdown. Format: "1950:2056"</summary>
    public string YearRange { get; set; } = "1950:2056";

    /// <summary>Date format string shown in the input. E.g. "MM/dd/yyyy"</summary>
    public string? DateFormat { get; set; }
}