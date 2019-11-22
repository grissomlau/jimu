using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Jimu.Client.RemoteCaller
{

    /// <summary>
    ///     Extension methods for adding typed middleware.
    /// </summary>
    public static class ClientCallerUseMiddlewareExtensions
    {
        internal const string InvokeMethodName = "Invoke";
        internal const string InvokeAsyncMethodName = "InvokeAsync";

        private static readonly MethodInfo GetServiceInfo =
            typeof(ClientCallerUseMiddlewareExtensions).GetMethod(nameof(GetService),
                BindingFlags.NonPublic | BindingFlags.Static);

        public static IRemoteServiceCaller UseMiddleware<TMiddleware>(this IRemoteServiceCaller app, params object[] args)
        {
            return app.UseMiddleware(typeof(TMiddleware), args);
        }


        public static IRemoteServiceCaller UseMiddleware(this IRemoteServiceCaller app, Type middleware, params object[] args)
        {
            return app.Use(next =>
            {
                var methods = middleware.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                var invokeMethods = methods.Where(m =>
                    string.Equals(m.Name, InvokeMethodName, StringComparison.Ordinal)
                    || string.Equals(m.Name, InvokeAsyncMethodName, StringComparison.Ordinal)
                ).ToArray();

                if (invokeMethods.Length > 1)
                    throw new InvalidOperationException($"Exception_UseMiddleMutlipleInvokes:{middleware.Name},{InvokeMethodName},{InvokeAsyncMethodName}");
                //Resources.FormatException_UseMiddleMutlipleInvokes(InvokeMethodName, InvokeAsyncMethodName));

                if (invokeMethods.Length == 0)
                    throw new InvalidOperationException($"Exception_UseMiddlewareNoInvokeMethod:{middleware.Name},{InvokeMethodName},{InvokeAsyncMethodName}");
                //Resources.FormatException_UseMiddlewareNoInvokeMethod(InvokeMethodName, InvokeAsyncMethodName));

                var methodinfo = invokeMethods[0];
                if (!typeof(Task<JimuRemoteCallResultData>).IsAssignableFrom(methodinfo.ReturnType))
                    throw new InvalidOperationException($"Exception_UseMiddlewareNonTaskReturnType:{middleware.Name},{InvokeMethodName},{InvokeAsyncMethodName},{nameof(Task<JimuRemoteCallResultData>)}");
                //Resources.FormatException_UseMiddlewareNonTaskReturnType(InvokeMethodName,
                //    InvokeAsyncMethodName, nameof(Task<JimuRemoteCallResultData>)));

                var parameters = methodinfo.GetParameters();
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(RemoteCallerContext) || parameters.Length > 1)
                    throw new InvalidOperationException($"Exception_UseMiddlewareNoParameters:{middleware.Name},{InvokeMethodName},{InvokeAsyncMethodName}");
                //Resources.FormatException_UseMiddlewareNoParameters(InvokeMethodName, InvokeAsyncMethodName,
                //    nameof(RemoteCallerContext)));


                var ctorArgs = new object[args.Length + 1];
                ctorArgs[0] = next;
                Array.Copy(args, 0, ctorArgs, 1, args.Length);
                var instance = ActivatorUtilities.CreateInstance(null, middleware, ctorArgs);

                return (ClientRequestDel)methodinfo.CreateDelegate(typeof(ClientRequestDel), instance);
            });
        }

        private static object GetService(IServiceProvider sp, Type type, Type middleware)
        {
            var service = sp.GetService(type);
            if (service == null)
                throw new InvalidOperationException($"Exception_InvokeMiddlewareNoService:{middleware.Name},{type},{middleware}");

            return service;
        }
    }
}