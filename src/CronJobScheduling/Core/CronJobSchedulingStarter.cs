namespace CronJobScheduling.Core;

public abstract class CronJobSchedulingStarter
{
    public static async Task StartAsync(IApplicationBuilder builder, CancellationToken cancellationToken = default)
    {
        //
        // Get cron jobs from application's service container.
        //
        var cronJobs = builder.ApplicationServices
            .GetServices<ICronJob>()
            .ToList();

        if (cronJobs.Count < 1)
        {
            return;
        }

        EnsureValidCronJobs(cronJobs);

        //
        // Get Quartz scheduler from application's service container.
        //
        var scheduler = await builder.ApplicationServices
            .GetRequiredService<ISchedulerFactory>()
            .GetScheduler(cancellationToken);

        //
        // Create list with jobs and their triggers for scheduler.
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
