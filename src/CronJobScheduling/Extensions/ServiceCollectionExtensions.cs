namespace CronJobScheduling.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCronJobScheduling(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            options.SchedulerId = "Scheduler.Core";
            options.SchedulerName = "Quartz.AspNetCore.Scheduler";

            options.UseMicrosoftDependencyInjectionJobFactory();

            options.MaxBatchSize = 5;
            options.InterruptJobsOnShutdown = true;
            options.InterruptJobsOnShutdownWithWait = true;
        });

        services.AddQuartzHostedService(options =>
        {
            options.StartDelay = TimeSpan.FromMilliseconds(1_000);
            options.AwaitApplicationStarted = true;
            options.WaitForJobsToComplete = true;
        });

        services.AddCronJobs(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }

    public static IServiceCollection AddCronJobs(this IServiceCollection services, Assembly assembly)
    {
        var abstraction = typeof(ICronJob);
        var baseType = typeof(CronJobBase<>);

        var implementations = assembly.GetTypes()
            .Where(t =>
                (t.IsAssignableTo(abstraction)
                 || t.BaseType == baseType)
                && t.IsClass
                && t.IsAbstract == false)
            .ToList();

        foreach (var implementation in implementations)
        {
            services.AddTransient(abstraction, implementation);
        }

        return services;
    }
}
