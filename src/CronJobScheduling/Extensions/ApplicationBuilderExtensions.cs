namespace CronJobScheduling.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder RunCronJobScheduling(this IApplicationBuilder builder)
    {
        CronJobSchedulingStarter.StartSchedulingAsync(builder).GetAwaiter();

        return builder;
    }
}
