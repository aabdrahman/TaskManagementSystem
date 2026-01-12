using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.User;

public record class UpdateUserDto
{
    [Required(ErrorMessage = "User Id is a required field")]
    [Range(1, int.MaxValue, ErrorMessage = "User Id is required")]
    public int Id { get; set; }
    [Required(ErrorMessage = "User Name is a required field.")]
    [StringLength(50, ErrorMessage = "User Name cannot exceed 50 characters.")]
    public string Username { get; set; }
    [EmailAddress(ErrorMessage = "The provided email is not valid.")]
    [Required(ErrorMessage = "Email Address is a required field.")]
    public string Email { get; set; }
    [Phone]
    [Required(ErrorMessage = "Phone Number is a required field.")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "First Name is a required field.")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Last Name is a required field.")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Assignd Unit is a required field.")]
    [Range(1, int.MaxValue, ErrorMessage = "Unit is required")]
    public int AssignedUnit { get; set; }
    [Required(ErrorMessage = "Assigned Role is a required field.")]
    [Range(1, int.MaxValue, ErrorMessage = "Role is required")]
    public int AssignedRole { get; set; }
}