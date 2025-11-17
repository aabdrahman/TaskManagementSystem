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
}
