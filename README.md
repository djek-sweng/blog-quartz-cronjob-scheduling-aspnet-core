### Quartz Cronjob Scheduling ASP.NET Core

Quartz.NET ist ein voll funktionsfähiges, quelloffenes Job Scheduling Framework, das von kleinsten Anwendungen bis hin zu großen Unternehmenssystemen eingesetzt werden kann.

Dieser Blogbeitrag zeigt dir, wie du Quartz.NET in deine eigene ASP.NET Core WebApi integrieren kannst.

Hauptaugenmerk liegt auf der Interaktion mit einem relationalen Datenbanksystem (hier Postgres) sowie mit Microsofts Objekt-Datenbank-Mapper Entity Framework Core (kurz EF Core).

#### **Vorteile**
Durch die Verwendung von Quartz bieten sich dir die folgenden Vorteile:

* Quartz kann in deine bestehende Anwendungen integriert oder als eigenständiges Programm ausgeführt werden.
* Ein ausführbarer Job ist eine einfache .NET-Klasse, die das `IJob` Interface von Quartz implementiert.
* Der Quartz Scheduler führt einen Job aus, wenn der zugehörige Trigger erfolgt. 
* Ein Trigger unterstützt eine Vielzahl von Optionen und ist über eine CronExpression sekundengenau einstellbar.
* Über die Implementierung eines Listeners können Scheduling-Ergebnisse überwacht werden.

#### **Einstieg**




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




#### **Fazit**
