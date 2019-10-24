using System;
using System.Collections.Generic;
using System.Text;
using Jimu;
using User.IService;

namespace User.Service
{
    public class RedirectService : IRedirectService
    {
        public JimuRedirect Go(string url)
        {
            return new JimuRedirect(url);
        }
    }
}
