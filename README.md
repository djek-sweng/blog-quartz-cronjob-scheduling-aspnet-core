### Quartz.NET Job Scheduling Framework - Integration into an ASP.NET Core WebApi

Almost every application needs them, services that perform certain background tasks. These services must operate independently, cyclically and detached from the main functionality of the application. A common approach to solving this task is provided by [cron jobs](https://en.wikipedia.org/wiki/Cron), which are known from [UNIX](https://en.wikipedia.org/wiki/Unix) or unixoid operating systems. The jobs are invoked centrally by a [scheduler](https://en.wikipedia.org/wiki/Job_scheduler).

[Quartz.NET](https://www.quartz-scheduler.net/) is a proven, open source and well documented job scheduling framework that can be used in a wide variety of applications.

This blog post shows you how to integrate Quartz.NET (Quartz for short) into your [ASP.NET Core](https://learn.microsoft.com/en-US/aspnet/core/) WebApi. In a proof of concept application, you'll test Quartz's interaction with a relational database system (here, [Postgres](https://www.postgresql.org/)) as well as with Microsoft's object database mapper [Entity Framework Core](https://docs.microsoft.com/en-us/ef/) (EF Core for short).

#### **Advantages**

Using quartz gives you the following advantages:

* Quartz can be integrated into your existing applications or run as a standalone program.
* An executable job is a class that implements a Quartz job interface.
* The Quartz scheduler executes a job when the associated trigger occurs.
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

In the file [`Directory.Build.targets`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/Directory.Build.targets) you will then find all the required packages you need to install for the application shown here.

#### **Configure Quartz**

Before the WebApi with integrated Quartz can be started, the Quartz services must be configured. You can find the configuration in the method [`AddCronJobScheduling()`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling/Extensions/ServiceCollectionExtensions.cs), which is called in the file [`Program.cs`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling.WebApi/Program.cs) as an extension of [`IServiceCollection`](https://learn.microsoft.com/de-de/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection).

```csharp
// File: CronJobScheduling.Extensions.ServiceCollectionExtensions.cs (excerpt)

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

