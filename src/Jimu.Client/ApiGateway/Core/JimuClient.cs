using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Jimu.Client;
using Jimu.Common.Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Jimu.Client.ApiGateway
{
    public class JimuClient
    {
        public static IServiceHost Host { get; set; }

        public static async Task<JimuRemoteCallResultData> Invoke(string path, IDictionary<string, object> paras)
        {
            var remoteServiceInvoker = Host.Container.Resolve<IRemoteServiceCaller>();
            var converter = Host.Container.Resolve<ISerializer>();
            var result = await remoteServiceInvoker.InvokeAsync(path, paras);
            if (!string.IsNullOrEmpty(result.ExceptionMessage))
            {
                throw new JimuHttpStatusCodeException(400, $"{result.ToErrorString()}", path);
            }

            if (!string.IsNullOrEmpty(result.ErrorCode) || !string.IsNullOrEmpty(result.ErrorMsg))
            {
                if (int.TryParse(result.ErrorCode, out int erroCode) && erroCode > 200 && erroCode < 600)
                {
                    throw new JimuHttpStatusCodeException(erroCode, result.ToErrorString(), path);
                }

                return new JimuRemoteCallResultData { ErrorCode = result.ErrorCode, ErrorMsg = result.ErrorMsg };
            }
            if (result.ResultType == typeof(JimuFile).ToString())
            {
                var file = converter.Deserialize(result.Result, typeof(JimuFile));
                result.Result = file;
            }

            return result;
        }

    }
}
