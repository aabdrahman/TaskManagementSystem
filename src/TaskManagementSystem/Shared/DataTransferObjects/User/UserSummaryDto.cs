namespace Shared.DataTransferObjects.User;

public record class UserSummaryDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
}