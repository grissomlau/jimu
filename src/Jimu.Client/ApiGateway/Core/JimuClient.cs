using Autofac;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jimu.Client.ApiGateway
{
    public class JimuClient
    {
        public static IApplication Host { get; set; }

        public static async Task<JimuRemoteCallResultData> Invoke(string path, IDictionary<string, object> paras, string httpMethod)
        {
            var remoteServiceInvoker = Host.Container.Resolve<IRemoteServiceCaller>();
            var result = await remoteServiceInvoker.InvokeAsync(path, paras, httpMethod);
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
            //if (result.ResultType == typeof(JimuFile).ToString())
            if (result?.ResultType != null && result.ResultType.StartsWith("{\"ReturnType\":\"Jimu.JimuRedirect\""))
            {
                var redirect = JimuHelper.Deserialize(result.Result, typeof(JimuRedirect));
                result.Result = redirect;
            }
            else if (result?.ResultType != null && result.ResultType.StartsWith("{\"ReturnType\":\"Jimu.JimuFile\""))
            {
                var file = JimuHelper.Deserialize(result.Result, typeof(JimuFile));
                result.Result = file;
            }

            return result;
        }

    }
}
