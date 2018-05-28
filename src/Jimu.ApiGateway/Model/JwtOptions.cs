using System;
using System.Linq;

namespace Jimu.ApiGateway.Model
{
    public class JwtOptions
    {
        public string SecretKey { get; set; }
        public bool ValidateIssuer { get; set; }
        public string ValidIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public string ValidAudience { get; set; }
        public bool ValidateLifetime { get; set; }
        public string ClockSkew { get; set; }
        public TimeSpan TimeSpanClockSkew
        {
            get
            {
                var timeArr = ClockSkew.Split(' ');
                if (timeArr.Any() && timeArr.Count() == 3)
                {
                    return new TimeSpan(int.Parse(timeArr[0]), int.Parse(timeArr[1]), int.Parse(timeArr[2]), 0);
                }

                return new TimeSpan();
            }

        }
    }
}
