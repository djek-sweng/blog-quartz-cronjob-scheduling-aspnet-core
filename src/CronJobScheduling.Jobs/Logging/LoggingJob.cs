namespace CronJobScheduling.Jobs.Logging;

public class LoggingJob : CronJobBase<LoggingJob>
{
    public override string Name => nameof(LoggingJob);
    public override string Group => CronGroupDefaults.User;
    public override string CronExpression => CronExpressionDefaults.EverySecondFrom0Through59;

    private readonly ILogger<LoggingJob> _logger;

    public LoggingJob(ILogger<LoggingJob> logger)
    {
        _logger = logger;
    }

    protected override Task InvokeAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{UtcNow}] {Name} is executed", DateTime.UtcNow, Name);

        return Task.CompletedTask;
    }
}
