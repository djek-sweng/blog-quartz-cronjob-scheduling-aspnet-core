namespace CronJobScheduling.Abstractions;

public abstract class CronJobBase<T> : ICronJob
    where T : class
{
    public virtual string Name => typeof(T).FullName ?? nameof(T);
    public virtual string Description => string.Empty;

    public abstract string Group { get; }
    public abstract string CronExpression { get; }

    public async Task Execute(IJobExecutionContext context)
    {
        await InvokeAsync(context.CancellationToken);
    }

    protected abstract Task InvokeAsync(CancellationToken cancellationToken);
}
