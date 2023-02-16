namespace CronJobScheduling.Jobs;

public class SchedulerAliveJob : ICronJob
{
    public string Name => nameof(SchedulerAliveJob);
    public string Description => "Checking that scheduler is alive.";
    public string Group => CronGroupDefaults.Core;
    public string CronExpression => CronExpressionDefaults.Every10ThSecondFrom0Through59;

    private readonly ILogger<SchedulerAliveJob> _logger;

    public SchedulerAliveJob(ILogger<SchedulerAliveJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("[{UtcNow}] Cron job scheduler is alive", DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
