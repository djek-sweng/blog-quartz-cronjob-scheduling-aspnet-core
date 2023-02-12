namespace CronJobScheduling.Core;

public abstract class CronJobSchedulingStarter
{
    public static async Task StartSchedulingAsync(IApplicationBuilder builder, CancellationToken cancellationToken = default)
    {
        //
        // Create service scope to resolve injected application services.
        //
        using var scope = builder.ApplicationServices.CreateScope();

        //
        // Get cron jobs from service container.
        //
        var cronJobs = scope.ServiceProvider
            .GetServices<ICronJob>()
            .ToList();

        if (cronJobs.Count < 1)
        {
            return;
        }

        EnsureValidCronJobs(cronJobs);

        //
        // Get Quartz scheduler factory from service container and build scheduler.
        //
        var scheduler = await scope.ServiceProvider
            .GetRequiredService<ISchedulerFactory>()
            .GetScheduler(cancellationToken);

        //
        // Create dictionary with jobs and their triggers for scheduler.
        //
        var jobsAndTriggers = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>();

        foreach (var cronJob in cronJobs)
        {
            var jobDetail = JobBuilder.Create(cronJob.GetType())
                .WithIdentity(cronJob.Name, cronJob.Group)
                .WithDescription(cronJob.Description)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"trigger.{cronJob.Name}", "standard")
                .WithCronSchedule(cronJob.CronExpression)
                .Build();

            jobsAndTriggers.Add(jobDetail, new[] { trigger });
        }

        //
        // Finish scheduling.
        //
        await DeleteExistingJobFromScheduler(scheduler, jobsAndTriggers.Keys, cancellationToken);

        await scheduler.ScheduleJobs(jobsAndTriggers, replace: false, cancellationToken);
        await scheduler.Start(cancellationToken);
    }

    private static void EnsureValidCronJobs(IEnumerable<ICronJob> cronJobs)
    {
        foreach (var cronJob in cronJobs)
        {
            CronExpression.ValidateExpression(cronJob.CronExpression);
        }
    }

    private static async Task DeleteExistingJobFromScheduler(
        IScheduler scheduler,
        IEnumerable<IJobDetail> jobDetails,
        CancellationToken cancellationToken)
    {
        foreach (var jobDetail in jobDetails)
        {
            if (await scheduler.CheckExists(jobDetail.Key, cancellationToken))
            {
                await scheduler.DeleteJob(jobDetail.Key, cancellationToken);
            }
        }
    }
}
