namespace Entities.Models;

public class User : BaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }

    public DateTime? LastLoginDate { get; set; }
    public DateTime LastPasswordChangeDate { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpirationTime { get; set; }

    public bool IsActive { get; set; } = true;

    //RELATIONSHIPS
    /* Unit User Relationship Many-to-One */
    public int UnitId { get; set; }
    public Unit AssignedUnit { get; set; }

    /* User User Role Relationship One-to-many */
    public ICollection<UserRole> RoleLink { get; set; } = [];

    /* Task User Relationship One-to-many */
    public ICollection<TaskUser> TaskLink { get; set; } = [];

    /* Task */
    public ICollection<CreatedTask> CreatedTasks { get; set; } = [];
}
