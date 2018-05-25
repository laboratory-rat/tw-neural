using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public static class DateTimeExtensions
    {
        public static long ToTimestamp(this DateTime time)
        {
            return (long)(time.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static DateTime FromTimestampUtc(this DateTime time, long timestamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            time = dtDateTime.AddSeconds(timestamp).ToUniversalTime();
            return time;
        }
    }
}
