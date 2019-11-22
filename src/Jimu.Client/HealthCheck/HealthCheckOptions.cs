namespace Jimu.Client.HealthCheck
{
    public class HealthCheckOptions
    {
        public int IntervalMinute { get; set; }

        public HealthCheckOptions(int intervalMinute)
        {
            this.IntervalMinute = intervalMinute;
        }
        public HealthCheckOptions() { }
    }
}
