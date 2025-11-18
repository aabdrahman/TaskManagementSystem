using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Repository;

public class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<CreatedTask> CreatedTasks { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<TaskUser> TaskUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
