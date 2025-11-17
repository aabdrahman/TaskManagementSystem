namespace Entities.Models;

public class Roles : BaseEntity
{
    public string Name { get; set; }
    public string NormalizedName { get; set; }

    //RELATIONSHIPS
    /* User Roles Relationship */
    public ICollection<UserRole> UserLink { get; set; } = [];
}

