namespace Kara_OK.Web.Helpers;

public static class TimeHelpers
{
    public static DateTime CeilToQuarterHour(DateTime dt)
    {
        // Drop seconds and milliseconds
        dt = dt.AddSeconds(-dt.Second).AddMilliseconds(-dt.Millisecond);

        var mod = dt.Minute % 15;
        if (mod == 0) return dt;

        return dt.AddMinutes(15 - mod);
    }

    public static bool IsQuarterHour(DateTime dt)
        => dt.Second == 0 && dt.Millisecond == 0 && dt.Minute % 15 == 0;
}