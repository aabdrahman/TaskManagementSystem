using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Tests.TestArtifacts;

public class TestRepositoryContext : DbContext
{
    public TestRepositoryContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>().HasQueryFilter(x => !x.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }
}
