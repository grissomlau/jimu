using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jimu.Core.Bus
{
    public interface IJimuRequestHandler<in Req, Resp> where Req : IJimuRequest where Resp : class
    {
        Task<Resp> HandleAsync(IJimuRequestContext<Req> context);
    }
}
