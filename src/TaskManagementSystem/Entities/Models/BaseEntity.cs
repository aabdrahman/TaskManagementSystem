namespace Entities.Models;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.ToLocalTime();
    public string? CreatedBy { get; set; }
}
