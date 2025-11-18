using Contracts.Infrastructure;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace TaskManagementSystem.Api.ServiceExtensions;

internal static class ApplicationServiceExtensions
{
    internal static void AddCorsImplementation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(opts =>
        {
            opts.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
        });
    }

    internal static void ConfigureSwagger(this IServiceCollection services)
    {

        services.AddSwaggerGen(opts =>
        {
            opts.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "TaskManagementSystemApi", Description = "Task Management System Api", Version = "v1", Contact = new Microsoft.OpenApi.Models.OpenApiContact()
            {
                Name = "Abdrahman Akande",
                Email = "",
                Url = new Uri("https://github.com/aabdrahman/TaskManagementSystem")
            } });
        });

    }

    internal static void ConfigureSqlDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RepositoryContext>(opts =>
        {
            opts.UseSqlServer(configuration.GetConnectionString("DbConnection"))
                .EnableSensitiveDataLogging();
        });
    }

    internal static void ConfigureLogging(this IServiceCollection services)
    {
        services.AddScoped<ILoggerManager, LoggerManager>();
    }
}
