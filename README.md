### Quartz.NET Job Scheduling Framework - Integration into an ASP.NET Core WebApi

Almost every application needs them, services that perform certain background tasks. These services must operate independently, cyclically and detached from the main functionality of the application. A common approach to solving this task is provided by [cron jobs](https://en.wikipedia.org/wiki/Cron), which are known from [UNIX](https://en.wikipedia.org/wiki/Unix) or unixoid operating systems. The jobs are invoked centrally by a [scheduler](https://en.wikipedia.org/wiki/Job_scheduler).

[Quartz.NET](https://www.quartz-scheduler.net/) is a proven, open source and well documented job scheduling framework that can be used in a wide variety of applications.

This blog post shows you how to integrate Quartz.NET (Quartz for short) into your [ASP.NET Core](https://learn.microsoft.com/en-US/aspnet/core/) WebApi. In a proof of concept application, you'll test Quartz's interaction with a relational database system (here, [Postgres](https://www.postgresql.org/)) as well as with Microsoft's object database mapper [Entity Framework Core](https://docs.microsoft.com/en-us/ef/) (EF Core for short).

