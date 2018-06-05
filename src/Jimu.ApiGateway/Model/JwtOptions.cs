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
        public int ValidMinute { get; set; }
        public TimeSpan TimeSpanClockSkew => new TimeSpan(0, ValidMinute, 0);
    }
}
