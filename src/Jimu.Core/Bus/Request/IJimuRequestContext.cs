using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Core.Bus
{
    public interface IJimuRequestContext<out T> where T : IJimuRequest
    {
        T Message { get; }
        IJimuBus JimuBus { get; }
    }
}
