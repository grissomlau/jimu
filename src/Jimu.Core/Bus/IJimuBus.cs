using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jimu.Core.Bus
{
    public interface IJimuBus
    {
        /// <summary>
        /// send an command to specify queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">command </param>
        Task SendAsync<T>(T command) where T : IJimuCommand;

        /// <summary>
        /// publish an event to all queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event">event</param>
        Task PublishAsync<T>(T @event) where T : IJimuEvent;

        /// <summary>
        /// send an request and expect response
        /// </summary>
        /// <typeparam name="Req">request</typeparam>
        /// <typeparam name="Resp">response</typeparam>
        /// <param name="request"></param>
        /// <param name="timeout">timeout before receive the response</param>
        /// <returns></returns>
        Task<Resp> RequestAsync<Req, Resp>(Req request, TimeSpan timeout = default, CancellationToken cancellationToken = default) where Req : class, IJimuRequest where Resp : class;
    }
}
