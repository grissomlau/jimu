using System;

namespace Jimu.ApiGateway.Utils
{
    internal static class Extensions
    {
        public static int ToInt(this DateTime time)
        {
            int result = 0;
            DateTime startdate = new DateTime(1970, 1, 1, 8, 0, 0);
            TimeSpan seconds = time.AddDays(1) - startdate;
            result = Convert.ToInt32(seconds.TotalSeconds);
            return result;
        }

        public static DateTime ToDate(this Int64 time)
        {
            Int64 bigtime = time * 10000000;
            DateTime dtOf1970 = new DateTime(1970, 1, 1, 8, 0, 0);
            long tricksOf1970 = dtOf1970.Ticks;
            long timeTricks = tricksOf1970 + bigtime;
            DateTime dt = new DateTime(timeTricks);
            DateTime enddt = dt.Date;
            //var inttime = ToInt(dt);
            return enddt;
        }

    }
}
