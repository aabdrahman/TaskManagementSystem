using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects.User;

public class UserToLoginDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    [PasswordPropertyText]
    public string Password { get; set; }
}
