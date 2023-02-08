namespace CronJobScheduling;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCronJobScheduling(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            options.SchedulerId = "MySchedulerId";
            options.SchedulerName = "MySchedulerName";

            // var loggerFactory = new LoggerFactory()
            //     .AddSerilog(Log.Logger);
            // options.SetLoggerFactory(loggerFactory);

            options.UseMicrosoftDependencyInjectionJobFactory();

            options.MaxBatchSize = 5;
            options.InterruptJobsOnShutdown = true;
            options.InterruptJobsOnShutdownWithWait = true;
        });

        services.AddQuartzHostedService(options =>
        {
            options.AwaitApplicationStarted = true;
            options.StartDelay = TimeSpan.FromSeconds(10);
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
