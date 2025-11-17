using System.Runtime.CompilerServices;

namespace Contracts.Infrastructure;

public interface ILoggerManager
{
    Task LogInfo(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
    Task LogWarning(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
    Task LogError(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
    Task LogDebug(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
    Task LogCritical(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
}
