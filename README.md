### Quartz.NET Job Scheduling Framework - Integration into an ASP.NET Core WebApi

Almost every application needs them, services that perform certain background tasks. These services must operate independently, cyclically and detached from the main functionality of the application. A common approach to solving this task is provided by [cron jobs](https://en.wikipedia.org/wiki/Cron), which are known from [UNIX](https://en.wikipedia.org/wiki/Unix) or unixoid operating systems. The jobs are invoked centrally by a [scheduler](https://en.wikipedia.org/wiki/Job_scheduler).

[Quartz.NET](https://www.quartz-scheduler.net/) is a proven, open source and well documented job scheduling framework that can be used in a wide variety of applications.

This blog post shows you how to integrate Quartz.NET (Quartz for short) into your [ASP.NET Core](https://learn.microsoft.com/en-US/aspnet/core/) WebApi. In a proof of concept application, you will test Quartz's interaction with a relational database system (in this case [Postgres](https://www.postgresql.org/)) as well as with Microsoft's object database mapper [Entity Framework Core](https://docs.microsoft.com/en-us/ef/) (EF Core for short).

#### **Advantages**

Using Quartz gives you the following advantages:

* Quartz can be integrated into your existing applications or run as a standalone program.
* An executable job is a class that implements a specific Quartz job interface.
* The Quartz scheduler executes a job, when the associated trigger occurs.
* A trigger supports a variety of options and can be adjusted to the second via a [cron expression](https://www.freeformatter.com/cron-expression-generator-quartz.html).
* Scheduling results can be monitored via the implementation of a listener.

#### **Install Quartz**

The shell script [`dotnet_add_quartz.sh`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/tools/dotnet/dotnet_add_quartz.sh) shows you how to install Quartz in your project environment.

```sh
#!/bin/sh

# File: dotnet_add_quartz.sh

dotnet add package Microsoft.Extensions.Hosting
dotnet add package Quartz
dotnet add package Quartz.Extensions.DependencyInjection
dotnet add package Quartz.Extensions.Hosting
```

In the file [`Directory.Build.targets`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/Directory.Build.targets) you will then find all required packages you need to install for the application shown here.

#### **Configure Quartz**

Before the WebApi with integrated Quartz can be started, the Quartz services must be configured. You can find the configuration in the method [`AddCronJobScheduling()`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling/Extensions/ServiceCollectionExtensions.cs), which is called in the file [`Program.cs`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling.WebApi/Program.cs) as an extension of [`IServiceCollection`](https://learn.microsoft.com/de-de/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection).

```csharp
// File: ServiceCollectionExtensions.cs (excerpt)

namespace CronJobScheduling.Extensions;

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

    return services;
}
```

The following configurations in detail:

* `UseMicrosoftDependencyInjectionJobFactory()`: Integrates job instantiation using the Microsoft dependency injection system.
* `MaxBatchSize`: The maximum number of triggers that a scheduler node is allowed to acquire (for firing) at once. Default value is 1.

The other settings are self-explanatory and can be found in the Quartz [documentation](https://www.quartz-scheduler.net/documentation).

#### **Create ICronJob interface**

The jobs implemented in the blog are to be executed as a cron job using cron expression. For this purpose, the interface [`ICronJob`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling/Abstractions/ICronJob.cs) is created, which extends the Quartz standard interface `IJob`.

```csharp
// File: ICronJob.cs

namespace CronJobScheduling.Abstractions;

public interface ICronJob : IJob
{
    string Name { get; }
    string Group { get; }
    string CronExpression { get; }
    string Description => string.Empty;
}
```

Besides the `CronExpression` the implementation of `ICronJob` requires a `Name`, a `Group` and an optional `Description`. All properties are needed later when scheduling the cron job.

You can find examples of valid cron expressions in the class [`CronExpressionDefaults`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling/Core/CronExpressionDefaults.cs) or in the Quartz [documentation](https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#example-cron-expressions). You can also find a cron expression generator and explainer for Quartz on the homepage [freeformater.com](https://www.freeformatter.com/cron-expression-generator-quartz.html).

#### **Create abstract base class CronJobBase**

To avoid direct dependency of cron jobs on `ICronJob` and `IJob` or on Quartz itself, create the abstract base class [`CronJobBase`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling/Abstractions/CronJobBase.cs).

```csharp
// File: CronJobBase.cs

namespace CronJobScheduling.Abstractions;

public abstract class CronJobBase<T> : ICronJob
    where T : class
{
    public virtual string Name => typeof(T).FullName ?? nameof(T);
    public virtual string Description => string.Empty;

    public abstract string Group { get; }
    public abstract string CronExpression { get; }

    public async Task Execute(IJobExecutionContext context)
    {
        await InvokeAsync(context.CancellationToken);
    }

    protected abstract Task InvokeAsync(CancellationToken cancellationToken);
}
```

The base class implements the method `Execute()` required by the interface `IJob`. Inside `Execute()` the method `InvokeAsync()` is called, which must be implemented by the deriving child class. Inside `InvokeAsync()` the actual functionality of the cron job is then embedded.

In the following section you implement cron jobs as child classes of `CronJobBase`.

#### **Implement cron jobs**

For the application example, you implement two cron jobs below, which create and delete records in a Postgres database via the `DbContext` of EF Core.

The first cron job [`CreateNoteJob`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling.Jobs/DataStore/CreateNoteJob.cs) creates a new [`Note`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling.DataStore/Models/Note.cs) record with each call and stores it in the database.

```csharp
// File: CreateNoteJob.cs

namespace CronJobScheduling.Jobs.DataStore;

public class CreateNoteJob : CronJobBase<CreateNoteJob>
{
    public override string Description => "Creates one note each time it is executed.";
    public override string Group => CronGroupDefaults.User;
    public override string CronExpression => CronExpressionDefaults.Every5ThSecondFrom0Through59;

    private readonly INoteRepository _noteRepository;

    public CreateNoteJob(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    protected override async Task InvokeAsync(CancellationToken cancellationToken)
    {
        var note = Note.Create($"Created by '{Name}' at '{DateTime.UtcNow}'.");

        await _noteRepository.AddNoteAsync(note, cancellationToken);
    }
}
```

The cron job `CreateNoteJob` should be executed every five seconds, see `Every5ThSecondFrom0Through59`.

To create and store a `Note` record, `CreateNoteJob` uses an implementation of [`INoteRepository`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling.DataStore/Repositories/Interfaces/INoteRepository.cs), which is accessed via the standard constructor injection.

The implementations of [`NoteRepository`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling.DataStore/Repositories/NoteRepository.cs) and [`ApplicationDbContext`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling.DataStore/Data/ApplicationDbContext.cs) are registered in the method [`AddCronJobSchedulingDataStore()`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling.DataStore/Extensions/ServiceCollectionExtensions.cs) in the WebHost's service container.

The second cron job [`DeleteNotesJob`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling.Jobs/DataStore/DeleteNotesJob.cs) deletes all `Note` records except the last two `Note` records with each call.

```csharp
// File: DeleteNotesJob.cs

namespace CronJobScheduling.Jobs.DataStore;

public class DeleteNotesJob : CronJobBase<DeleteNotesJob>
{
    public override string Description => "Deletes all notes except the two latest notes.";
    public override string Group => CronGroupDefaults.User;
    public override string CronExpression => CronExpressionDefaults.EveryMinuteAtSecond0;

    private readonly INoteRepository _noteRepository;

    public DeleteNotesJob(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    protected override async Task InvokeAsync(CancellationToken cancellationToken)
    {
        var notes = await _noteRepository.GetNotesDescendingAsync(skip: 2, cancellationToken);

        await _noteRepository.RemoveNotesAsync(notes, cancellationToken);
    }
}
```

The cron job `DeleteNotesJob` should be executed every full minute, see `EveryMinuteAtSecond0`.

You can look at two other cron job implementations in the classes [`LoggingJob`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling.Jobs/Logging/LoggingJob.cs) and [`SchedulerAliveJob`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling/Jobs/SchedulerAliveJob.cs).

#### **Register cron jobs in the service container**

Before adding the cron jobs to the scheduler, you register them in the service container of the WebHost. This is done automatically using the [`AddCronJobs()`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling/Extensions/ServiceCollectionExtensions.cs) method.

```csharp
// File: ServiceCollectionExtensions.cs (excerpt)

namespace CronJobScheduling.Extensions;

public static IServiceCollection AddCronJobs(
    this IServiceCollection services,
    Assembly assembly)
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
```

The method uses `Reflection` to register all cron jobs of the given `Assembly`.

#### **Start cron job scheduling**

In the last step, the Quartz Scheduler is populated with the implemented cron jobs and started. This is done automatically via the method [`StartSchedulingAsync()`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling/Core/CronJobSchedulingStarter.cs).

```csharp
// File: CronJobSchedulingStarter.cs (excerpt)

public static async Task StartSchedulingAsync(
    IApplicationBuilder builder,
    CancellationToken cancellationToken = default)
{
    //
    // Create service scope to resolve injected application services.
    //
    using var scope = builder.ApplicationServices.CreateScope();

    //
    // Get cron jobs from service container.
    //
    var cronJobs = scope.ServiceProvider
        .GetServices<ICronJob>()
        .ToList();

    if (cronJobs.Count < 1)
    {
        return;
    }

    EnsureValidCronJobs(cronJobs);

    //
    // Get Quartz scheduler factory from service container and build scheduler.
    //
    var scheduler = await scope.ServiceProvider
        .GetRequiredService<ISchedulerFactory>()
        .GetScheduler(cancellationToken);

    //
    // Create dictionary with jobs and their triggers for scheduler.
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
```

The cron jobs and the Quartz scheduler factory are obtained from the service container. The scheduler factory creates the scheduler, which is then populated with a dictionary of pairs of jobs and triggers. The triggers are configured with the associated CronExpressions. Finally, the scheduler is started.

After the Quartz scheduler has been started, the WebApi can also be started. The last two lines of the file [`Program.cs`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling.WebApi/Program.cs) start the scheduler and WebApi.

```csharp
// File: Program.cs (excerpt)

app.RunCronJobScheduling();

app.Run();
```

#### **Execute application example (proof of concept)**

If you have [Docker](https://www.docker.com/) installed on your machine, you can run the Postgres database server used in the application inside a Docker container. Simply start the Docker Engine and then run the shell script [`run_npgsql_server.sh`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/run_npgsql_server.sh).

You can then use the following connection string to connect the applications to the database:

```
Server=localhost; Port=4200; Username=root; Password=pasSworD; Database=cronjob_db;
```

If you have a Postgres database server installed on your computer, you can also use it. In this case, ensure an appropriate configuration.

Afterwards start the WebApi by executing the shell script [`run_webapi.sh`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/run_webapi.sh).

If you use the shell script `run_npgsql_server.sh` for the application example, then you can open the database [Adminer](https://www.adminer.org/en/) in your browser using the following URL http://localhost:4300.

A look into the database table `Notes` shows that every five seconds a new `Note` record is created and stored. At every full minute, all `Note` records are then deleted except for the last two `Note` records.

Furthermore, a look into the terminal of the WebApi shows that all four cron job implementations are executed.

#### **Conclusion**

The blog post shows you the full integration of the Quartz.NET framework into your ASP.NET Core WebApi. Implementing a cron job is easy. For this you have created an abstract base class. The configuration of the Quartz services, as well as the starting of the scheduler is fully automated. For this you have written appropriate extension methods.

In a further step you could monitor the scheduling results. For this purpose, the implementation of appropriate Quartz listeners is a good idea. You could also persist the execution of the individual cron jobs in a corresponding database table. For this you could adapt the cron job base class. Both extensions would improve the quality of your job scheduling system significantly.

You can find the complete code in this GitHub repository.

Happy Coding!

German version

[https://www.traperto.com/bla-bla-blog/](https://www.traperto.com/bla-bla-blog/)
