using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.Unit;

public record class CreateUnitDto
{
    [Required(ErrorMessage = "Unit Name is a required field.")]
    [StringLength(maximumLength:50, ErrorMessage = "Name cannot exceed 50 characters.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Unit Head Name is a required field.")]
    [StringLength(maximumLength: 50, ErrorMessage = "Unit Head Name cannot exceed 50 characters.")]
    public string UnitHeadName { get; set; }
}
