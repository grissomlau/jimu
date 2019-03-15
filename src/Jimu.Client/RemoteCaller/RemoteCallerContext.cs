using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client
{
    public class RemoteCallerContext
    {
        public JimuServiceRoute Service { get; }
        public IDictionary<string, object> Paras { get; }

        public string Token { get; }
        public JimuPayload PayLoad { get; set; }

        public RemoteCallerContext(JimuServiceRoute service, IDictionary<string, object> paras, string token, JimuPayload payload)
        {
            this.Service = service;
            this.Paras = paras;
            this.Token = token;
            this.PayLoad = payload;
        }

        public RemoteCallerContext(JimuServiceRoute service, IDictionary<string, object> paras, string token)
        {
            this.Service = service;
            this.Paras = paras;
            this.Token = token;
        }
    }
}
