using Crabshell.Core.Attributes;
using Crabshell.Core.Registry;

namespace Crabshell.Core.Services;

public static class CollectionValidator
{
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
                if (!opts.Contains(value))
                    errors.Add(new ValidationError(field.PropertyName,
                        $"{field.Label} must be one of: {string.Join(", ", opts)}."));
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

            if (field.FieldType == FieldType.DateTime)
            {
                if (!System.DateTime.TryParse(value, out _))
                    errors.Add(new ValidationError(field.PropertyName,
                        $"{field.Label} must be a valid date/time."));
            }
        }

        return errors;
    }
}