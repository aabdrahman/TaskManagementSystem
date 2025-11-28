using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.User;

public record class ChangeUserPasswordDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email format.")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
    [Required(ErrorMessage = "New Password is required.")]
    public string NewPassword { get; set; }
    [Required(ErrorMessage = "Confirm New Password is required.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Password Mismatch.")]
    public string ConfirmNewPassword { get; set; }
}
