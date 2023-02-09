### Quartz.NET Cron-Job Scheduling ASP.NET Core

Quartz.NET ist ein voll bewährtes, quelloffenes und gut dokumentiertes Job-Scheduling Framework, das in verschiedensten Anwendungen eingesetzt werden kann.

Dieser Blogbeitrag zeigt dir, wie du Quartz.NET (kurz Quartz) in eine ASP.NET Core WebApi integrieren kannst.

Hauptaugenmerk liegt auf der Interaktion mit einem relationalen Datenbanksystem (hier Postgres) sowie mit Microsofts Objekt-Datenbank-Mapper Entity Framework Core (kurz EF Core).

#### **Vorteile**
Durch die Verwendung von Quartz bieten sich dir die folgenden Vorteile:

* Quartz kann in deine bestehende Anwendungen integriert oder als eigenständiges Programm ausgeführt werden.
* Ein ausführbarer Job ist eine Klasse, die das `IJob` Interface von Quartz implementiert.
* Der Quartz Scheduler führt einen Job aus, wenn der zugehörige Trigger erfolgt. 
* Ein Trigger unterstützt eine Vielzahl von Optionen und ist über eine Cron-Expression sekundengenau einstellbar.
* Über die Implementierung eines Listeners können Scheduling-Ergebnisse überwacht werden.

#### **Quartz installieren**
Das Shell-Skript [`dotnet_add_quartz.sh`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/tools/dotnet/dotnet_add_quartz.sh) zeigt dir, wie du Quartz in deiner Projektumgebung installierst.

```sh
#!/bin/sh

# File: dotnet_add_quartz.sh

dotnet add package Microsoft.Extensions.Hosting
dotnet add package Quartz
dotnet add package Quartz.Extensions.DependencyInjection
dotnet add package Quartz.Extensions.Hosting
```

In der Datei [`Directory.Build.targets`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/Directory.Build.targets) findest du dann alle notwendigen Pakete, die du für die hier gezeigte Anwendung installieren musst.

#### **Quartz konfigurieren**
Bevor die WebApi mit integriertem Quartz gestartet werden kann, müssen die Quartz Services konfiguriert werden. Du findest die Konfiguration in der Methode `AddCronJobScheduling()`, welche in der Datei `Program.cs` als Erweiterung von `IServiceCollection` aufgerufen wird.

```csharp
// File: CronJobScheduling.Extensions.ServiceCollectionExtensions.cs (Auszug)

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

#### **Interface ICronJob erstellen**
Die im Blog implementierten Jobs sollen als Cron-Job mittels Cron-Expression ausgeführt werden. Dafür wird das Interface [`ICronJob`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling/Abstractions/ICronJob.cs) erstellt, welches das Quartz Standard-Interface `IJob` erweitert. 

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

Neben der `CronExpression` erfordert die Implementierung von `ICronJob` einen `Name`, eine `Group` und eine optionale `Description`.

Beispiele für gültige Cron-Expressions findest du in der Klasse [`CronExpressionDefaults`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling/Core/CronExpressionDefaults.cs) oder in der Quartz [Dokumentation](https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#example-cron-expressions).

#### **Abstrakte Basisklasse CronJobBase**
Um eine direkte Abhängigkeit der Cron-Jobs auf `ICronJob` und `IJob` bzw. auf Quartz selbst zu vermeiden wird die abstrakte Basisklasse [`CronJobBase`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/src/CronJobScheduling/Abstractions/CronJobBase.cs) implementiert.


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

Die Basisklasse implementiert die von `IJob` geforderte Methode `Execute(IJobExecutionContext context)`



Die im Blog implementierten Jobs 



:

:

:

:

:

:

:

:

:

:

:

:

:

:

:

:

#### **Job implementieren**

















#### **Job dem Scheduler hinzufügen**






#### **Scheduler starten**




#### **Postgres Datenbankserver starten**
Sofern du [Docker](https://www.docker.com/) auf deinem Rechner installiert hast, kannst du den in der Anwendung verwendenten Postgres Datenbankserver innerhalb eines Docker Containers ausführen. Starte dafür einfach den Docker Engine und führe anschließend das Shell-Skript [`run_npgsql_server.sh`](https://github.com/djek-sweng/blog-quartz-cronjob-scheduling-aspnet-core/blob/main/run_npgsql_server.sh) aus.

Über den folgenden Connection String kannst du die Anwendungen dann mit der Datenbank verbinden:

```
Server=localhost; Port=4200; Username=root; Password=pasSworD; Database=cronjob_db;
```

Wenn du auf deinem Rechner einen Postgres Datenbankserver installiert hast, dann kannst du auch diesen verwenden. Stelle in diesem Fall aber eine entsprechende Konfiguration sicher.




#### **Fazit**
