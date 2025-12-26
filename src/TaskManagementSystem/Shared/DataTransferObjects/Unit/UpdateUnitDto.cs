using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.Unit;

public record class UpdateUnitDto
{
    [Required(ErrorMessage = "Id is a required field.")]
    [Range(1, maximum: double.MaxValue, ErrorMessage = "Id must be greater than zero.")]
    public int UnitId { get; set; }
    [Required(ErrorMessage = "Unit Name is a required field.")]
    [StringLength(maximumLength: 50, ErrorMessage = "Name cannot exceed 50 characters.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Unit Head Name is a required field.")]
    [StringLength(maximumLength: 50, ErrorMessage = "Unit Head Name cannot exceed 50 characters.")]
    public string UnitHeadName { get; set; }
}