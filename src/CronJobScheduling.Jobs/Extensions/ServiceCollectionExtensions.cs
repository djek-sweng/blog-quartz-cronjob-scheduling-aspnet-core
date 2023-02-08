namespace CronJobScheduling.Jobs.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCronJobSchedulingJobs(this IServiceCollection services)
    {
        services.AddCronJobs(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}
