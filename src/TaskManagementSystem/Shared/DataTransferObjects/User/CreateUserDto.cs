namespace Shared.DataTransferObjects.User;

public record class CreateUserDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public int AssignedUnit { get; set; }
    public int AssignedRole { get; set; }
}
