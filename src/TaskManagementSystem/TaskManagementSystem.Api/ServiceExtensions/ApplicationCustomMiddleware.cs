using Swashbuckle.AspNetCore.SwaggerUI;

namespace TaskManagementSystem.Api.ServiceExtensions;

internal static class ApplicationCustomMiddleware
{
    internal static void ConfigureSwaggerDefinition(this WebApplication app)
    {
        app.UseSwagger(opts =>
        {
            opts.RouteTemplate = "TaskManagementSystemAPI/swagger/{documentname}/swagger.json";
        });

        app.UseSwaggerUI(opts => 
        {
            opts.RoutePrefix = "TaskManagementSystemAPI/swagger";
            opts.SwaggerEndpoint("/TaskManagementSystemAPI/swagger/v1/swagger.json", "TaskManagementSystemApi");
        });
    }

    internal static void CongigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(opts => { });
    }
}
