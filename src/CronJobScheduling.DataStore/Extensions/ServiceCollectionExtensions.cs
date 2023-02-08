namespace CronJobScheduling.DataStore.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCronJobSchedulingDataStore(
        this IServiceCollection services,
        Assembly migrationsAssembly,
        string? connectionString)
    {
        if (migrationsAssembly is null)
        {
            throw new ArgumentException("Migrations assembly must not be null.");
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string must not be null, empty or white space.");
        }

        var assemblyName = migrationsAssembly.FullName;

        if (string.IsNullOrWhiteSpace(assemblyName))
        {
            throw new ArgumentException("Migrations assembly name must not be null, empty or white space.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, opt => opt.MigrationsAssembly(assemblyName)));

        services.AddScoped<INoteRepository, NoteRepository>();

        return services;
    }
}
