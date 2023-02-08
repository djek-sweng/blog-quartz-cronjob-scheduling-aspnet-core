namespace CronJobScheduling.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder RunCronJobScheduling(this IApplicationBuilder builder)
    {
        CronJobSchedulingStarter.StartAsync(builder).GetAwaiter();

        return builder;
    }
}
