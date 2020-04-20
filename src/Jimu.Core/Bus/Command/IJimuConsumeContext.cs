using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Core.Bus
{
    public interface IJimuConsumeContext<out T> where T : IJimuCommand
    {
        T Message { get; }
        IJimuBus JimuBus { get; }
    }
}
