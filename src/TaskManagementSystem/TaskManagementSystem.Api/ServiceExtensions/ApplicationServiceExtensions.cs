using Contracts;
using Contracts.Infrastructure;
using Entities.ConfigurationModels;
using Infrastructure;
using Infrastructure.Contracts;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Service.Contract;
using Services;
using Shared.ApiResponse;
using System.Text;
using System.Text.Json;

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

            opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "Enter  your Bearer token here. Enter 'Bearer' followed by your token (e.g., Bearer abc.def.ghi)",
                In = ParameterLocation.Header,
                Scheme = "Bearer",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT"
            });

            opts.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer"
                    },
                    new List<string>()
                }
            });
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

    internal static void ConfigureInfrastrucureManager(this IServiceCollection services)
    {
        services.AddScoped<IInfrastructureManager, InfrastructureManager>();
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

    internal static void ConfigureHttpContextAccessor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
    }
    internal static void ConfigureModelsFromSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<UploadConfig>(configuration.GetSection("UploadConfiguration"));
        services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));
        //services.AddOptions<UploadConfig>("UploadConfiguration").Bind(configuration).ValidateDataAnnotations();
    }

    internal static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = configuration.GetSection("JwtSettings").Get<JwtConfiguration>();

        services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,

                ValidIssuer = jwtConfig.validIssuer,
                ValidAudiences = jwtConfig.validAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Secret-Key"] ?? throw new ArgumentNullException("Secret Key Required.")))
            };
            //opts.Events = new JwtBearerEvents()
            //{
            //    OnForbidden = async (context) =>
            //    {
            //        context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //        context.Response.ContentType = "application/json";

            //        var response = GenericResponse<string>.Failure("Unauthorzied Access", System.Net.HttpStatusCode.Forbidden, "Not Authorized", null);


            //        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            //    },
            //    OnAuthenticationFailed = async (context) =>
            //    {
            //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //        context.Response.ContentType = "application/json";

            //        var response = GenericResponse<string>.Failure("Unauthorzied Access", System.Net.HttpStatusCode.Unauthorized, "Not Authenticated", null);


            //        await context.Response.WriteAsync(JsonSerializer.Serialize(response));


            //    },

            //};
        });
    }
}
