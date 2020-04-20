using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Core.Bus
{
    public interface IJimuSubscribeContext<out T> where T : IJimuEvent
    {
        T Message { get; }
        IJimuBus JimuBus { get; }
    }
}
