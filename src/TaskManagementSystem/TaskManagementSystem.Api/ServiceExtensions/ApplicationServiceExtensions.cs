using Contracts;
using Contracts.Infrastructure;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Repository;
using Service.Contract;
using Services;

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
                Email = "akandeabdrahman@gmail.com",
                Url = new Uri("https://github.com/aabdrahman/TaskManagementSystem")
            } });
        });

    }

    internal static void ConfigureSqlDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RepositoryContext>(opts =>
        {
            opts.UseSqlServer(configuration.GetConnectionString("DbConnection"))
                .EnableSensitiveDataLogging()
                .LogTo(
                    Serilog.Log.Information,
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information,
                    DbContextLoggerOptions.SingleLine);
        });
    }

    internal static void ConfigureLogging(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerManager, LoggerManager>();
    }

    internal static void ConfigureRepositoryManager(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryManager, RepositoryManager>();
    }

    internal static void ConfigureServiceManager(this IServiceCollection services)
    {
        services.AddScoped<IServiceManager, ServiceManager>();
    }

    internal static void ConfigureController(this IServiceCollection services)
    {
        services.AddControllers(opts =>
        {

        }).AddApplicationPart(typeof(TaskManagementSystem.ApiPresentation.AssemblyReference).Assembly);
    }

    internal static void ConfigureExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
    }
}
