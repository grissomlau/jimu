using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.APM
{
    public class ApmClientType
    {
        public const string ListenerName = "JimuClientDiagnosticListener";
        public const string RpcExecuteBefore = "JimuClient.RpcExecuteBefore";
        public const string RpcExecuteAfter = "JimuClient.RpcExecuteAfter";
        public const string RpcExecuteError = "JimuClient.RpcExecuteError";

    }
}
