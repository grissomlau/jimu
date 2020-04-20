using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
