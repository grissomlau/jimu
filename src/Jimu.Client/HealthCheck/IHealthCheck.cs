using System;
using System.Threading.Tasks;

namespace Jimu.Client
{
    /// <summary>
    ///     server health check monitor
    /// </summary>
    public interface IHealthCheck : IDisposable
    {
        Task RunAsync();
    }
}