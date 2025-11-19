using Contracts.Infrastructure;
using Serilog;
using System.Runtime.CompilerServices;

namespace LoggerService;

public class LoggerManager : ILoggerManager
{
    private ILogger _logger = Log.ForContext<LoggerManager>();
    private readonly string _methodDefinitionName = "MethodName";
    private readonly string _classDefinitionName = "ClassName";

    public Task LogCritical(Exception ex, string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
    {
        string className = Path.GetFileNameWithoutExtension(callerFile);

        return Task.FromResult(
               () =>
               {
                   _logger.ForContext(_classDefinitionName, className)
                   .ForContext(_methodDefinitionName, callerName)
                   .Fatal(ex, message);
               }
            );
    }

    public Task LogDebug(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
    {
        string className = Path.GetFileNameWithoutExtension(callerFile);

        return Task.FromResult
        (
            () =>
            {
                _logger.ForContext(_classDefinitionName, className)
                        .ForContext(_methodDefinitionName, callerName)
                        .Debug(message);
            }   
        );
    }

    public Task LogError(Exception ex, string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
    {
        string className = Path.GetFileNameWithoutExtension(callerFile);

        return Task.FromResult(() =>
        {
            _logger
            .ForContext(_classDefinitionName, className)
            .ForContext(_methodDefinitionName, callerName)
            .Error(ex, message);
        });
    }

    public Task LogInfo(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
    {
        string className = Path.GetFileNameWithoutExtension(callerFile);

        return Task.FromResult(() =>
        {
            _logger
                .ForContext(_classDefinitionName, className)
                .ForContext(_methodDefinitionName, callerName)
                .Information(message);
        });
    }

    public Task LogWarning(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "")
    {
        string className = Path.GetFileNameWithoutExtension(callerFile);

        return Task.FromResult(() =>
        {
            _logger
                .ForContext(_classDefinitionName, className)
                .ForContext(_methodDefinitionName, callerName)
                .Warning(message);
        });
    }
}
