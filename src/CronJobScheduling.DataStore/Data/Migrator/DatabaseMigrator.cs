namespace CronJobScheduling.DataStore.Data.Migrator;

public class DatabaseMigrator
{
    private readonly IApplicationBuilder _applicationBuilder;

    private DatabaseMigrator(IApplicationBuilder applicationBuilder)
    {
        _applicationBuilder = applicationBuilder;
    }

    public static DatabaseMigrator CreateMigrator(IApplicationBuilder applicationBuilder)
    {
        return new DatabaseMigrator(applicationBuilder);
    }

    public void Migrate<T>()
        where T : DbContext
    {
        using var scope = _applicationBuilder.ApplicationServices.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<T>();

        context.Database.Migrate();
    }
}
