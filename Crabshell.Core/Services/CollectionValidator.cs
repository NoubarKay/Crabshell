using Crabshell.Core.Attributes;
using Crabshell.Core.Registry;

namespace Crabshell.Core.Services;

/// <summary>Validates raw form values against the field rules declared on a collection.</summary>
public static class CollectionValidator
{
    /// <summary>
    /// Validates <paramref name="formValues"/> against the field rules in <paramref name="collection"/>.
    /// Returns an empty list when all values are valid, or one <see cref="ValidationError"/> per failing field.
    /// </summary>
    public static List<ValidationError> Validate(
        CollectionMeta collection,
        Dictionary<string, string?> formValues)
    {
        var errors = new List<ValidationError>();

        foreach (var field in collection.Fields)
        {
            formValues.TryGetValue(field.PropertyName, out var value);

            if (field.Required && string.IsNullOrWhiteSpace(value))
            {
                errors.Add(new ValidationError(field.PropertyName, $"{field.Label} is required."));
                continue;
            }

            if (value is null) continue;

            if (field.FieldType == FieldType.Text && field.TextSettings is { } text)
            {
                if (text.MinLength > 0 && value.Length < text.MinLength)
                    errors.Add(new ValidationError(field.PropertyName,
                        $"{field.Label} must be at least {text.MinLength} characters."));

                if (text.MaxLength != -1 && value.Length > text.MaxLength)
                    errors.Add(new ValidationError(field.PropertyName,
                        $"{field.Label} must be at most {text.MaxLength} characters."));

                if (!string.IsNullOrEmpty(text.Pattern) && !System.Text.RegularExpressions.Regex.IsMatch(value, text.Pattern))
                    errors.Add(new ValidationError(field.PropertyName,
                        $"{field.Label} is not in the correct format."));
            }

            if (field.FieldType == FieldType.Number && field.NumberSettings is { } num)
            {
                if (decimal.TryParse(value, out var numVal))
                {
                    if (num.Min.HasValue && numVal < num.Min.Value)
                        errors.Add(new ValidationError(field.PropertyName,
                            $"{field.Label} must be at least {num.Min.Value}."));

                    if (num.Max.HasValue && numVal > num.Max.Value)
                        errors.Add(new ValidationError(field.PropertyName,
                            $"{field.Label} must be at most {num.Max.Value}."));
                }
            }

            if (field.FieldType == FieldType.Select && field.SelectSettings?.Options is { Length: > 0 } opts)
            {
                var tokens = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var invalid = tokens.Where(t => !opts.Contains(t)).ToList();
                if (invalid.Count > 0)
                    errors.Add(new ValidationError(field.PropertyName,
                        $"{field.Label} contains invalid option(s): {string.Join(", ", invalid)}."));
            }

            if (field.FieldType == FieldType.Relationship)
            {
                if (!Guid.TryParse(value, out _))
                    errors.Add(new ValidationError(field.PropertyName,
                        $"{field.Label} must be a valid reference."));
            }

            if (field.FieldType == FieldType.Bool)
            {
                if (!bool.TryParse(value, out _))
                    errors.Add(new ValidationError(field.PropertyName,
                        $"{field.Label} must be true or false."));
            }

            if (field is { FieldType: FieldType.DateTime, DateTimeSettings: { } dt })
            {
                if (!DateTimeOffset.TryParse(value, out var parsed))
                {
                    errors.Add(new ValidationError(field.PropertyName,
                        $"{field.Label} must be a valid date/time."));
                }
                else
                {
                    if (dt.Min is not null && DateTimeOffset.TryParse(dt.Min, out var min) && parsed < min)
                        errors.Add(new ValidationError(field.PropertyName,
                            $"{field.Label} must be on or after {dt.Min}."));
                    if (dt.Max is not null && DateTimeOffset.TryParse(dt.Max, out var max) && parsed > max)
                        errors.Add(new ValidationError(field.PropertyName,
                            $"{field.Label} must be on or before {dt.Max}."));
                }
            }
        }

        return errors;
    }
}