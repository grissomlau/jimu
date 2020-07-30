using System;

namespace Jimu.Common
{
    public static class Extensions
    {
        public static long ToLong(this DateTime time)
        {
            //var startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            var startTime = new DateTime(1970, 1, 1);
            return (long)(time.ToUniversalTime() - startTime).TotalSeconds;
        }

        public static DateTime ToDate(this long time)
        {
            //var dateTimeStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            var dateTimeStart = new DateTime(1970, 1, 1);
            var lTime = long.Parse(time + "0000000");
            var toNow = new TimeSpan(lTime);

            return dateTimeStart.Add(toNow).ToLocalTime();
        }

        public static string ToStackTraceString(this Exception exception)
        {
            if (exception == null)
                return string.Empty;
            return GetExceptionMessage(exception);
        }

        private static string GetExceptionMessage(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var message = $"{exception.Message} StackTrace:{exception.StackTrace}";
            if (exception.InnerException != null)
                message += "|InnerException:" + GetExceptionMessage(exception.InnerException);
            return message;
        }
    }
}