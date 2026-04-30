namespace Crabshell.Core.Attributes.Fields;

/// <summary>
/// Maps to an integer, numeric, or double precision column depending on the CLR property type.
/// int/long → integer/bigint | decimal → numeric(18, Decimals) | double/float → double precision
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class NumberFieldAttribute : CrabshellFieldAttribute
{
    /// <summary>Decimal places stored. Only relevant for decimal properties. Default 2.</summary>
    public int Decimals { get; set; } = 2;

    /// <summary>Minimum allowed value. Omit to leave unconstrained.</summary>
    public double Min { get; set; } = double.NaN;

    /// <summary>Maximum allowed value. Omit to leave unconstrained.</summary>
    public double Max { get; set; } = double.NaN;

    /// <summary>Spinner increment step in the admin UI. Default 1.</summary>
    public string Step { get; set; } = 1.ToString();

    /// <summary>Text shown before the input (e.g. "$"). Admin UI only.</summary>
    public string? Prefix { get; set; }

    /// <summary>Text shown after the input (e.g. "%"). Admin UI only.</summary>
    public string? Suffix { get; set; }
    
    public string? Format { get; set; }
}