using Repository;

namespace TaskManagementSystem.Tests.TestArtifacts;

public class TestRepository : RepositoryBase<TestEntity>
{
    public TestRepository(RepositoryContext context)
        : base(context) { }
}
