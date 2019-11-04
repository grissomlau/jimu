using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Jimu.Server
{

    /// <summary>
    ///     Extension methods for adding typed middleware.
    /// </summary>
    public static class ServerUseMiddlewareExtensions
    {
        internal const string InvokeMethodName = "Invoke";
        internal const string InvokeAsyncMethodName = "InvokeAsync";

        private static readonly MethodInfo GetServiceInfo =
            typeof(ServerUseMiddlewareExtensions).GetMethod(nameof(GetService),
                BindingFlags.NonPublic | BindingFlags.Static);

        public static IServer UseMiddleware<TMiddleware>(this IServer app, params object[] args)
        {
            return app.UseMiddleware(typeof(TMiddleware), args);
        }


        public static IServer UseMiddleware(this IServer app, Type middleware, params object[] args)
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

                if (invokeMethods.Length == 0)
                    throw new InvalidOperationException($"Exception_UseMiddlewareNoInvokeMethod:{middleware.Name},{InvokeMethodName},{InvokeAsyncMethodName}");

                var methodinfo = invokeMethods[0];
                if (!typeof(Task).IsAssignableFrom(methodinfo.ReturnType))
                    throw new InvalidOperationException($"Exception_UseMiddlewareNonTaskReturnType:{middleware.Name},{InvokeMethodName},{InvokeAsyncMethodName}");

                var parameters = methodinfo.GetParameters();
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(ServiceInvokerContext))
                    throw new InvalidOperationException($"Exception_UseMiddlewareNoParameters:{middleware.Name},{InvokeMethodName},{InvokeAsyncMethodName}");


                var ctorArgs = new object[args.Length + 1];
                ctorArgs[0] = next;
                Array.Copy(args, 0, ctorArgs, 1, args.Length);
                var instance = ActivatorUtilities.CreateInstance(null, middleware, ctorArgs);

                if (parameters.Length == 1)
                    return (RequestDel)methodinfo.CreateDelegate(typeof(RequestDel), instance);

                var factory = Compile<object>(methodinfo, parameters);

                return context => factory(instance, context, null);
            });
        }

        private static Func<T, ServiceInvokerContext, IServiceProvider, Task> Compile<T>(MethodInfo methodinfo,
            ParameterInfo[] parameters)
        {
            var middleware = typeof(T);

            var httpContextArg = Expression.Parameter(typeof(ServiceInvokerContext), "httpContext");
            var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
            var instanceArg = Expression.Parameter(middleware, "middleware");

            var methodArguments = new Expression[parameters.Length];
            methodArguments[0] = httpContextArg;
            for (var i = 1; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                if (parameterType.IsByRef)
                    throw new NotSupportedException($"Exception_InvokeDoesNotSupportRefOrOutParams:{middleware.Name},{InvokeMethodName}");

                var parameterTypeExpression = new Expression[]
                {
                    providerArg,
                    Expression.Constant(parameterType, typeof(Type)),
                    Expression.Constant(methodinfo.DeclaringType, typeof(Type))
                };

                var getServiceCall = Expression.Call(GetServiceInfo, parameterTypeExpression);
                methodArguments[i] = Expression.Convert(getServiceCall, parameterType);
            }

            Expression middlewareInstanceArg = instanceArg;
            if (methodinfo.DeclaringType != typeof(T))
                middlewareInstanceArg = Expression.Convert(middlewareInstanceArg, methodinfo.DeclaringType);

            var body = Expression.Call(middlewareInstanceArg, methodinfo, methodArguments);

            var lambda =
                Expression.Lambda<Func<T, ServiceInvokerContext, IServiceProvider, Task>>(body, instanceArg,
                    httpContextArg, providerArg);

            return lambda.Compile();
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