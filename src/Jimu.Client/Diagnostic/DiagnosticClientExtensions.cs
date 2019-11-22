using Jimu.Diagnostic;
using Jimu.Client.Diagnostic.EventData;
using Jimu.Client.RemoteCaller;
using System;
using System.Runtime.CompilerServices;

namespace Jimu.Client.Diagnostic
{
    public static class DiagnosticClientExtensions
    {


        #region RPCExecute


        public static Guid WriteRPCExecuteBefore(this IJimuDiagnostic @this, RemoteCallerContext remoteCallerContext
            , [CallerMemberName] string operation = ""
            )
        {
            if (!@this.IsEnabled(DiagnosticClientEventType.RpcExecuteBefore)) return Guid.Empty;
            var operationId = Guid.NewGuid();

            @this.Write(DiagnosticClientEventType.RpcExecuteBefore, new RPCExecuteBeforeEventData(operationId, operation) { Data = remoteCallerContext });

            return operationId;
        }

        public static void WriteRPCExecuteAfter(this IJimuDiagnostic @this, Guid operationId, RemoteCallerContext context, JimuRemoteCallResultData resultData
            , [CallerMemberName] string operation = ""
            )
        {
            if (@this.IsEnabled(DiagnosticClientEventType.RpcExecuteAfter))
            {
                @this.Write(DiagnosticClientEventType.RpcExecuteAfter, new RPCExecuteAfterEventData(operationId, operation)
                {
                    Data = context,
                    ResultData = resultData
                });

            }
        }

        public static void WriteRPCExecuteError(this IJimuDiagnostic @this, Guid operationId, RemoteCallerContext remoteCallerContext, Exception exception
            , [CallerMemberName] string operation = ""
            )
        {
            if (@this.IsEnabled(DiagnosticClientEventType.RpcExecuteError))
            {
                @this.Write(DiagnosticClientEventType.RpcExecuteError, new RPCExecuteErrorEventData(operationId, operation)
                {
                    Data = remoteCallerContext,
                    Ex = exception
                });
            }
        }

        #endregion


    }
}
