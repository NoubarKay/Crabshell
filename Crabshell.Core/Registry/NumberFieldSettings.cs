namespace Crabshell.Core.Registry;

public sealed class NumberFieldSettings
{
    public int Decimals { get; init; }
    public decimal? Min { get; init; }
    public decimal? Max { get; init; }
    public string Step { get; init; }
    public string? Prefix { get; init; }
    public string? Suffix { get; init; }
    public string? Format { get; init; }
}