namespace CronJobScheduling.Extensions;

public static class ServiceCollectionExtensions
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
