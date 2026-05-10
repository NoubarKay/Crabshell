namespace Crabshell.Core.Registry;

/// <summary>Runtime settings derived from <see cref="Attributes.Fields.DateTimeFieldAttribute"/>.</summary>
public sealed class DateTimeFieldSettings
{
    /// <summary>Include a time component (<c>timestamptz</c>). False = date only (<c>date</c>).</summary>
    public bool HasTime { get; init; } = false;

    /// <summary>Show only a time picker; no date calendar (<c>timetz</c> column).</summary>
    public bool TimeOnly { get; init; } = false;

    /// <summary>Store value as UTC.</summary>
    public bool Utc { get; init; } = true;

    /// <summary>Minimum allowed date in <c>yyyy-MM-dd</c> format, or <c>null</c> for no minimum.</summary>
    public string? Min { get; init; }

    /// <summary>Maximum allowed date in <c>yyyy-MM-dd</c> format, or <c>null</c> for no maximum.</summary>
    public string? Max { get; init; }

    /// <summary>Show a "Now" shortcut button in the picker.</summary>
    public bool ShowNowButton { get; init; } = false;

    /// <summary>Hour increment in the time picker.</summary>
    public int HoursStep { get; init; } = 1;

    /// <summary>Minute increment in the time picker.</summary>
    public int MinutesStep { get; init; } = 1;

    /// <summary>Second increment in the time picker.</summary>
    public int SecondsStep { get; init; } = 1;

    /// <summary>Update value on every keystroke rather than on blur.</summary>
    public bool Immediate { get; init; } = false;

    /// <summary>Show the calendar inline (always visible) rather than as a popup.</summary>
    public bool Inline { get; init; } = false;

    /// <summary>Show the calendar icon button.</summary>
    public bool ShowButton { get; init; } = true;

    /// <summary>Year range shown in the year dropdown, format <c>"min:max"</c>.</summary>
    public string YearRange { get; init; } = "1950:2056";

    /// <summary>Display format string for the date input, e.g. <c>"MM/dd/yyyy"</c>.</summary>
    public string? DateFormat { get; init; }
}
