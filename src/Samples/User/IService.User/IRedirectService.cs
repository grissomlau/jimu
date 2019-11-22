using Jimu;
using System;
using System.Collections.Generic;
using System.Text;

namespace IService.User
{
    [Jimu("/{Service}")]
    public interface IRedirectService : IJimuService
    {
        [JimuGet(true)]
        JimuRedirect Go(string url);
    }
}
