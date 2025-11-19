using System.Runtime.CompilerServices;

namespace Contracts.Infrastructure;

public interface ILoggerManager
{
    Task LogInfo(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
    Task LogWarning(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
    Task LogError(Exception ex, string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
    Task LogDebug(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
    Task LogCritical(Exception ex, string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFile = "");
}
