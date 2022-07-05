using Autofac;
using Jimu.Client.RemoteCaller;
using Jimu.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jimu.Client.ApiGateway
{
    public class JimuClient
    {
        public static IApplication Host { get; set; }

        static IRemoteServiceCaller _caller;
        public static IRemoteServiceCaller Caller
        {
            get
            {
                if (_caller == null)
                {
                    _caller = Host.Container.Resolve<IRemoteServiceCaller>();
                }
                return _caller;
            }
        }

        public static async Task<bool> Exist(string path, string httpMethod)
        {
            return await Caller.ExistAsync(path, httpMethod, null);
        }
        public static async Task<bool> Exist(string path, string httpMethod, IDictionary<string, object> paras)
        {
            return await Caller.ExistAsync(path, httpMethod, paras);
        }


        public static async Task<JimuRemoteCallResultData> Invoke(string path, IDictionary<string, object> paras, string httpMethod)
        {
            var result = await Caller.InvokeAsync(path, paras, httpMethod);
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
