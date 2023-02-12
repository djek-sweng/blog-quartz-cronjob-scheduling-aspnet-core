namespace CronJobScheduling.DataStore.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCronJobSchedulingDataStore(this IApplicationBuilder builder)
    {
        var migrator = DatabaseMigrator.CreateMigrator(builder);

        migrator.Migrate<ApplicationDbContext>();

        return builder;
    }
}
