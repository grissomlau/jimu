using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jimu.Client.Diagnostics
{
    public static class JimuClientDiagnosticListenerExtensions
    {
        public const string JIMU_CLIENT_DIAGNOSTIC_LISTENER = "JimuClientDiagnosticListner";
        public static readonly DiagnosticListener Listener = new DiagnosticListener(JIMU_CLIENT_DIAGNOSTIC_LISTENER);
        public const string JIMU_CLIENT_PREFIX = "JimuClient.";

        #region RPCExecute

        public const string JIMU_CLIENT_BEFORE_PRC_EXECUTE = JIMU_CLIENT_PREFIX + nameof(WriteRPCExecuteBefore);
        public const string JIMU_CLIENT_AFTER_PRC_EXECUTE = JIMU_CLIENT_PREFIX + nameof(WriteRPCExecuteAfter);
        public const string JIMU_CLIENT_ERROR_PRC_EXECUTE = JIMU_CLIENT_PREFIX + nameof(WriteRPCExecuteError);

        public static Guid WriteRPCExecuteBefore(this DiagnosticListener @this, RemoteCallerContext remoteCallerContext)
        {
            if (!@this.IsEnabled(JIMU_CLIENT_BEFORE_PRC_EXECUTE)) return Guid.Empty;
            var operationId = Guid.NewGuid();
            var operation = $"RPC: {remoteCallerContext.Service.ServiceDescriptor.Id}";

            @this.Write(JIMU_CLIENT_BEFORE_PRC_EXECUTE, new RPCExecuteBeforeEventData(operationId, operation) { Context = remoteCallerContext });

            return operationId;
        }

        public static void WriteRPCExecuteAfter(this DiagnosticListener @this, Guid operationId, RemoteCallerContext context, JimuRemoteCallResultData resultData
            //, [CallerMemberName] string operation = ""
            )
        {
            if (@this.IsEnabled(JIMU_CLIENT_AFTER_PRC_EXECUTE))
            {
                var operation = $"RPC: {context.Service.ServiceDescriptor.Id}";
                @this.Write(JIMU_CLIENT_AFTER_PRC_EXECUTE, new RPCExecuteAfterEventData(operationId, operation)
                {
                    Context = context,
                    ResultData = resultData
                });

            }
        }

        public static void WriteRPCExecuteError(this DiagnosticListener @this, Guid operationId, RemoteCallerContext remoteCallerContext, Exception exception)
        {
            if (@this.IsEnabled(JIMU_CLIENT_ERROR_PRC_EXECUTE))
            {
                var operation = $"RPC: {remoteCallerContext.Service.ServiceDescriptor.Id}";
                @this.Write(JIMU_CLIENT_ERROR_PRC_EXECUTE, new RPCExecuteErrorEventData(operationId, operation)
                {
                    Context = remoteCallerContext,
                    Ex = exception
                });
            }
        }

        #endregion
    }
}
