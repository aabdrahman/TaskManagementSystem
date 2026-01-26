using Contracts.Infrastructure;
using Quartz;

namespace TaskManageemntSystem.WorkerService.CustomScheduler;

public sealed class TestSchedulerJob : IJob
{
    private ILoggerManager _loggerManager;
    public TestSchedulerJob(ILoggerManager loggerManager)
    {
        _loggerManager = loggerManager;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _loggerManager.LogInfo($"Test Sceduled Quartz Job running at: {DateTime.Now}");
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, $"Error eccorred running the Test Scheduled Quartz Job at: {DateTime.Now}");
        }
    }
}
