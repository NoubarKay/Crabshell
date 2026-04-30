namespace Crabshell.Core.Registry;

public sealed class DateTimeFieldSettings
{
    public bool HasTime { get; init; } = false;
    public bool TimeOnly { get; init; } = false;
    public bool Utc { get; init; } = true;
    public string? Min { get; init; }
    public string? Max { get; init; }
    public bool ShowNowButton { get; init; } = false;
    public int HoursStep { get; init; } = 1;
    public int MinutesStep { get; init; } = 1;
    public int SecondsStep { get; init; } = 1;
    public bool Immediate { get; init; } = false;
    public bool Inline { get; init; } = false;
    public bool ShowButton { get; init; } = true;
    public string YearRange { get; init; } = "1950:2056";
    public string? DateFormat { get; init; }
}
