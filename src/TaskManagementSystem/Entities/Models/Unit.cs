namespace Entities.Models;

public class Unit : BaseEntity
{
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public string UnitHeadName { get; set; }

    //RELATIONSHIPS
    /* User Unit Relationship One-to-many */
    public ICollection<User> Users { get; set; } = [];
}