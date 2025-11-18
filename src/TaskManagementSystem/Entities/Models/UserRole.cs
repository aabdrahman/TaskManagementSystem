namespace Entities.Models;

public class UserRole : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public User user { get; set; }
    public Role role { get; set; }
}
