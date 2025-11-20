using System.ComponentModel.DataAnnotations;

namespace Shared.CustomValidator;

public class EnumTypeValidatorAttribute : ValidationAttribute
{
    private readonly Type _enumType;
    private string? _message;

    public EnumTypeValidatorAttribute(Type enumType, string? message = null)
    {
        if (enumType is null)
            throw new ArgumentNullException(nameof(enumType), "Enum cannot be null.");

        _enumType = enumType;
        _message = message;
    }
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not string)
            return new ValidationResult("The provided value must be a valid string");

        if(Enum.TryParse(_enumType, value.ToString(), ignoreCase: true, out _))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(string.IsNullOrEmpty(_message) ? $"Invalid Value provided" : _message);
    }
}