using TaskManagementSystem.Api.ServiceExtensions;
using Serilog;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);

string logFolderPath = string.IsNullOrEmpty(builder.Configuration.GetValue<string>("LogFilePath")) ? 
                                        Path.Combine(AppContext.BaseDirectory, "Logs") : 
                                        builder.Configuration.GetValue<string>("LogFilePath") 
                                ?? throw new ArgumentNullException("Log file path could not be determined.");

//Serilog Logger Configuration
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.File(Path.Combine(logFolderPath, "log-.txt"), Serilog.Events.LogEventLevel.Debug, 
                            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.ff zzz}||{Level:u3}] || [{ClassName}].[{MethodName}] - {Message:lj}{NewLine}{Exception}{Properties}{NewLine}", 
                            fileSizeLimitBytes: 10_000_000, rollOnFileSizeLimit: true,
                            rollingInterval: RollingInterval.Day)
            .CreateLogger();

// Add services to the container.

builder.Services.ConfigureSwagger();
builder.Services.AddCorsImplementation(builder.Configuration);
builder.Services.ConfigureLogging();
builder.Services.ConfigureExceptionHandler();
builder.Services.ConfigureSqlDbConnection(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureInfrastrucureManager();
builder.Services.ConfigureModelsFromSettings(builder.Configuration);
builder.Services.ConfigureController();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureSwaggerDefinition();

app.CongigureExceptionHandler();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "Attachments")
        ),
    RequestPath = new PathString("/Uploads")
});

app.UseCors();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

