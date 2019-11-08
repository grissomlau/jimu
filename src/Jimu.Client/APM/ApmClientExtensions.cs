using Jimu.APM;
using Jimu.Client.APM.EventData;
using Jimu.Client.RemoteCaller;
using System;
using System.Runtime.CompilerServices;

namespace Jimu.Client.APM
{
    public static class ApmClientExtensions
    {


        #region RPCExecute


        public static Guid WriteRPCExecuteBefore(this IJimuApm @this, RemoteCallerContext remoteCallerContext
            , [CallerMemberName] string operation = ""
            )
        {
            if (!@this.IsEnabled(ApmClientEventType.RpcExecuteBefore)) return Guid.Empty;
            var operationId = Guid.NewGuid();

            @this.Write(ApmClientEventType.RpcExecuteBefore, new RPCExecuteBeforeEventData(operationId, operation) { Data = remoteCallerContext });

            return operationId;
        }

        public static void WriteRPCExecuteAfter(this IJimuApm @this, Guid operationId, RemoteCallerContext context, JimuRemoteCallResultData resultData
            , [CallerMemberName] string operation = ""
            )
        {
            if (@this.IsEnabled(ApmClientEventType.RpcExecuteAfter))
            {
                @this.Write(ApmClientEventType.RpcExecuteAfter, new RPCExecuteAfterEventData(operationId, operation)
                {
                    Data = context,
                    ResultData = resultData
                });

            }
        }

        public static void WriteRPCExecuteError(this IJimuApm @this, Guid operationId, RemoteCallerContext remoteCallerContext, Exception exception
            , [CallerMemberName] string operation = ""
            )
        {
            if (@this.IsEnabled(ApmClientEventType.RpcExecuteError))
            {
                @this.Write(ApmClientEventType.RpcExecuteError, new RPCExecuteErrorEventData(operationId, operation)
                {
                    Data = remoteCallerContext,
                    Ex = exception
                });
            }
        }

        #endregion


    }
}
