using Jimu;
using System;
using System.Collections.Generic;
using System.Text;

namespace User.IService
{
    [JimuServiceRoute("/{Service}")]
    public interface IRedirectService : IJimuService
    {
        [JimuGet(true)]
        JimuRedirect Go(string url);
    }
}
