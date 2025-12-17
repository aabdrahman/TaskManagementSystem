namespace Shared.DataTransferObjects.User;

public record class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int DaysToPasswordExpiry { get; set; }
    public DateTime LastLoginDate { get; set; }
}
