using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Repository;

namespace TaskManagementSystem.Api.ContextFactory;

public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    public RepositoryContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json")
                                  .Build();

        var contextOptions = new DbContextOptionsBuilder<RepositoryContext>()
                                        .UseSqlServer(configuration.GetConnectionString("DbConnection") ?? throw new InvalidOperationException("No Database Connection string provided."), b => b.MigrationsAssembly("TaskManagementSystem.Api"))
                                        .EnableSensitiveDataLogging()
                                        .LogTo(
                                                Serilog.Log.Information,
                                                new[] { DbLoggerCategory.Database.Command.Name },
                                                LogLevel.Information,
                                                DbContextLoggerOptions.SingleLine
                                            );

        return new RepositoryContext(contextOptions.Options);

    }
}
