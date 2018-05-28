using System;

namespace Jimu.Core.Commons.Utils
{
    public static class Extensions
    {
        public static int ToInt(this DateTime time)
        {
            var startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return (int) (time - startTime).TotalSeconds;
        }

        public static DateTime ToDate(this long time)
        {
            var dateTimeStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            var lTime = long.Parse(time + "0000000");
            var toNow = new TimeSpan(lTime);

            return dateTimeStart.Add(toNow);
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