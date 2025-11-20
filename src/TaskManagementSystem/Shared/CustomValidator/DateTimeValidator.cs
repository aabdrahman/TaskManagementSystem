using System.ComponentModel.DataAnnotations;

namespace Shared.CustomValidator;

public class DateTimeValidatorAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is null || value is not DateTime)
        {
            return new ValidationResult("The provided value must be a valid datetime");
        }

        DateTime validDate = DateTime.Parse(value.ToString());

        if(validDate.Date <= DateTime.Today.Date)
        {
            return new ValidationResult("The provided date must be a future date.");
        }

        return ValidationResult.Success;
    }
}
