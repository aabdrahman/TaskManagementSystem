using TaskManagementSystem.Api.ServiceExtensions;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

string logFolderPath = builder.Configuration.GetValue<string>("LogFilePath") ?? Path.Combine(Directory.GetCurrentDirectory(), "Logs");

//Serilog Logger Configuration
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.File(Path.Combine(logFolderPath, "log-.txt"), Serilog.Events.LogEventLevel.Debug, 
                            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.ff zzz}||{Level:u3}] || [{ClassName}].[{MethodName}] - {Message:lj}{NewLine}{Exception}{eventId}", 
                            fileSizeLimitBytes: 10_000_000, rollOnFileSizeLimit: true,
                            rollingInterval: RollingInterval.Day)
            .CreateLogger();

// Add services to the container.

builder.Services.ConfigureSwagger();
builder.Services.AddCorsImplementation(builder.Configuration);
builder.Services.ConfigureLogging();
builder.Services.ConfigureSqlDbConnection(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureSwaggerDefinition();

app.UseCors();

app.UseHttpsRedirection();

app.Run();

