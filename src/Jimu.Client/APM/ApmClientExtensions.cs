using Jimu.APM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jimu.Client.APM
{
    public static class ApmClientExtensions
    {


        #region RPCExecute


        public static Guid WriteRPCExecuteBefore(this IJimuApm @this, RemoteCallerContext remoteCallerContext
            , [CallerMemberName] string operation = ""
            )
        {
            if (!@this.IsEnabled(ApmClientType.RpcExecuteBefore)) return Guid.Empty;
            var operationId = Guid.NewGuid();

            @this.Write(ApmClientType.RpcExecuteBefore, new RPCExecuteBeforeEventData(operationId, operation) { Data = remoteCallerContext });

            return operationId;
        }

        public static void WriteRPCExecuteAfter(this IJimuApm @this, Guid operationId, RemoteCallerContext context, JimuRemoteCallResultData resultData
            , [CallerMemberName] string operation = ""
            )
        {
            if (@this.IsEnabled(ApmClientType.RpcExecuteAfter))
            {
                @this.Write(ApmClientType.RpcExecuteAfter, new RPCExecuteAfterEventData(operationId, operation)
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
            if (@this.IsEnabled(ApmClientType.RpcExecuteError))
            {
                @this.Write(ApmClientType.RpcExecuteError, new RPCExecuteErrorEventData(operationId, operation)
                {
                    Data = remoteCallerContext,
                    Ex = exception
                });
            }
        }

        #endregion


    }
}
