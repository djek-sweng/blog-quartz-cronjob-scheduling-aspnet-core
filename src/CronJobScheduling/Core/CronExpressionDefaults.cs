namespace CronJobScheduling.Core;

public static class CronExpressionDefaults
{
    // https://www.freeformatter.com/cron-expression-generator-quartz.html
    // https://www.quartz-scheduler.net/documentation/quartz-4.x/tutorial/crontrigger.html
    //
    // Field Name     Mandatory	Allowed Values	   Allowed Special Characters
    // Seconds        YES         0-59              , - * /
    // Minutes        YES         0-59              , - * /
    // Hours          YES         0-23              , - * /
    // Day of month   YES         1-31              , - * ? / L W
    // Month          YES         1-12 or JAN-DEC   , - * /
    // Day of week    YES         1-7 or SUN-SAT    , - * ? / L #
    // Year           NO          empty, 1970-2099  , - * /

    // every second
    public const string EverySecondFrom0Through59 = "0/1 * * * * ? *";
    public const string Every5ThSecondFrom0Through59 = "0/5 * * * * ? *";
    public const string Every10ThSecondFrom0Through59 = "0/10 * * * * ? *";

    // every minute
    public const string EveryMinuteAtSecond0 = "0 * * * * ? *";
    public const string Every5ThMinuteFrom0Through59 = "0 0/5 * * * ? *";

    // every hour
    // every day
    public const string EveryDayAt0300 = "0 0 3 * * ? *";
}
