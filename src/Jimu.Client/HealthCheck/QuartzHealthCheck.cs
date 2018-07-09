using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Spi;

namespace Jimu.Client.HealthCheck
{
    public class QuartzHealthCheck : IHealthCheck
    {
        private readonly ILogger _logger;
        private IScheduler _scheduler;
        private readonly IClientServiceDiscovery _serviceDiscovery;
        private readonly string _cron;
        private readonly int _timeout = 30000;

        public QuartzHealthCheck(ILogger logger, IClientServiceDiscovery serviceDiscovery, int intervalMinute)
        {
            if (intervalMinute == 0 || intervalMinute > 60)
            {
                throw new ArgumentOutOfRangeException($"intervalMinite must between 1 and 60, current is {intervalMinute}");
            }
            _cron = $"0 0/{intervalMinute} * * * ?";

            _logger = logger;
            _serviceDiscovery = serviceDiscovery;
        }

        public void Dispose()
        {
            _scheduler.Clear();
        }

        public async Task RunAsync()
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            IJobDetail jobDetail = JobBuilder.Create<MonitorJob>().WithIdentity("MonitorJob", "Jimu.Client.HealthCheck").Build();
            jobDetail.JobDataMap.Put("serviceDiscovery", _serviceDiscovery);
            jobDetail.JobDataMap.Put("logger", _logger);
            jobDetail.JobDataMap.Put("timeout", _timeout);
            IOperableTrigger trigger = new CronTriggerImpl("MonitorJob", "HealthCheck", _cron);
            await _scheduler.ScheduleJob(jobDetail, trigger);
            await _scheduler.Start();
        }

        [DisallowConcurrentExecution]
        private class MonitorJob : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                var serviceDiscovery = context.JobDetail.JobDataMap.Get("serviceDiscovery") as IClientServiceDiscovery;
                var logger = context.JobDetail.JobDataMap.Get("logger") as ILogger;
                logger.Debug("******* start check server health job *******");
                var timeout = (int)context.JobDetail.JobDataMap.Get("timeout");
                if (serviceDiscovery != null)
                {
                    var routes = (await serviceDiscovery.GetRoutesAsync()).ToList();
                    var servers = (from route in routes
                                   from address in route.Address
                                       //where address.IsHealth
                                   select address).Distinct().ToList();
                    foreach (var server in servers)
                    {
                        await CheckHealth(server, timeout);
                    }

                    await serviceDiscovery.UpdateServerHealthAsync(servers);

                }
            }

            private async Task CheckHealth(JimuAddress address, int timeout)
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { SendTimeout = timeout })
                {
                    try
                    {
                        await socket.ConnectAsync(address.CreateEndPoint());
                        address.IsHealth = true;
                    }
                    catch
                    {
                        address.IsHealth = false;
                    }
                }
            }
        }
    }
}
