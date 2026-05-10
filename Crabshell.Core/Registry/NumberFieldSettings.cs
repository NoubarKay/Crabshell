namespace Crabshell.Core.Registry;

/// <summary>Runtime settings derived from <see cref="Attributes.Fields.NumberFieldAttribute"/>.</summary>
public sealed class NumberFieldSettings
{
    /// <summary>Decimal places for <c>decimal</c> properties. Ignored for integer types.</summary>
    public int Decimals { get; init; }

    /// <summary>Minimum allowed value, or <c>null</c> for no constraint.</summary>
    public decimal? Min { get; init; }

    /// <summary>Maximum allowed value, or <c>null</c> for no constraint.</summary>
    public decimal? Max { get; init; }

    /// <summary>Spinner increment step in the admin UI.</summary>
    public string? Step { get; init; }

    /// <summary>Text shown before the input (e.g. <c>"$"</c>). Admin UI only.</summary>
    public string? Prefix { get; init; }

    /// <summary>Text shown after the input (e.g. <c>"%"</c>). Admin UI only.</summary>
    public string? Suffix { get; init; }

    /// <summary>Display format string applied to the rendered value.</summary>
    public string? Format { get; init; }
}