using Microsoft.Extensions.Logging;

namespace Jimu.Server.UnitOfWork.EF
{
    public class EFOptions
    {
        public bool Enable { get; set; } = true;

        public string ConnectionString { get; set; }

        public string EFProviderName { get; set; }

        public string OptionName { get; set; }

        public bool IsDefaultOption { get; set; } = true;
        public bool IsSupportTransaction { get; set; } = true;

        public string TableModelAssemblyName { get; set; }
        public bool OpenLogTrace { get; set; }
        /// <summary>
        /// log level, default debug 
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
    }
}
