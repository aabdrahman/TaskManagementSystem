using Contracts.Infrastructure;

namespace TaskManageemntSystem.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

                //if(await timer.WaitForNextTickAsync())
                //{
                    using var scope = _serviceScopeFactory.CreateAsyncScope();
                    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerManager>();
                    await loggerFactory.LogInfo($"Worker running at: {DateTime.Now}");
                //}

            }
            await Task.Delay(1000);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateAsyncScope();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerManager>();
            await loggerFactory.LogInfo($"Worker stopping at: {DateTime.Now}");
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateAsyncScope();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerManager>();
            await loggerFactory.LogInfo($"Worker starting at: {DateTime.Now}");
        }
    }
}
