using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    public class JimuRedirect
    {
        public JimuRedirect() { }
        public JimuRedirect(string url)
        {
            RedirectUrl = url;
        }
        public string RedirectUrl { get; set; }
    }
}
