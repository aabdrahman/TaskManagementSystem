using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repository;
using TaskManagementSystem.Tests.TestArtifacts;

namespace TaskManagementSystem.Tests.Model.Tests;

public class RepositoryBaseTests
{
    private RepositoryContext _testRepositoryContext;

    public RepositoryBaseTests()
    {
        var options = new DbContextOptionsBuilder<RepositoryContext>()
                                .UseInMemoryDatabase("TaskManagementSystem")
                                .Options;
        _testRepositoryContext = new RepositoryContext(options);
    }

    //[Fact]
    //public async Task RepositoryBase_FindAll()
    //{
    //    //Arrange
    //    var mockRepoManager = new Mock<RepositoryManager>();
    //    var repoManager = new RepositoryManager(mockRepoManager.Object);
    //    var roleRepo = new RoleRepository(_testRepositoryContext);
    //    Role roleToInsert = new Role() {  Id = 1, Name = "TestRole"};

    //    //Act
    //    await roleRepo.CreateRole(roleToInsert);
    //    mockRepoManager.
    //    var roles = roleRepo.FindAll(true, true);

    //    //Assert
    //    Assert.NotEmpty(roles);
    //    Assert.NotNull(roles);
    //    Assert.Equal(1, roles.Count());
    //}

    [Fact]
    public async Task RoleRepository_Create()
    {
        //Arrange

        //Act

        //Assert
    }
}
