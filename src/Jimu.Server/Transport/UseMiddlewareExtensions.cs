using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
                    throw new InvalidOperationException(
                        Resources.FormatException_UseMiddleMutlipleInvokes(InvokeMethodName, InvokeAsyncMethodName));

                if (invokeMethods.Length == 0)
                    throw new InvalidOperationException(
                        Resources.FormatException_UseMiddlewareNoInvokeMethod(InvokeMethodName, InvokeAsyncMethodName));

                var methodinfo = invokeMethods[0];
                if (!typeof(Task).IsAssignableFrom(methodinfo.ReturnType))
                    throw new InvalidOperationException(
                        Resources.FormatException_UseMiddlewareNonTaskReturnType(InvokeMethodName,
                            InvokeAsyncMethodName, nameof(Task)));

                var parameters = methodinfo.GetParameters();
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(RemoteCallerContext))
                    throw new InvalidOperationException(
                        Resources.FormatException_UseMiddlewareNoParameters(InvokeMethodName, InvokeAsyncMethodName,
                            nameof(RemoteCallerContext)));


                var ctorArgs = new object[args.Length + 1];
                ctorArgs[0] = next;
                Array.Copy(args, 0, ctorArgs, 1, args.Length);
                var instance = ActivatorUtilities.CreateInstance(null, middleware, ctorArgs);

                if (parameters.Length == 1)
                    return (RequestDel) methodinfo.CreateDelegate(typeof(RequestDel), instance);

                var factory = Compile<object>(methodinfo, parameters);

                return context => factory(instance, context, null);
            });
        }

        private static Func<T, RemoteCallerContext, IServiceProvider, Task> Compile<T>(MethodInfo methodinfo,
            ParameterInfo[] parameters)
        {
            var middleware = typeof(T);

            var httpContextArg = Expression.Parameter(typeof(RemoteCallerContext), "httpContext");
            var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
            var instanceArg = Expression.Parameter(middleware, "middleware");

            var methodArguments = new Expression[parameters.Length];
            methodArguments[0] = httpContextArg;
            for (var i = 1; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                if (parameterType.IsByRef)
                    throw new NotSupportedException(
                        Resources.FormatException_InvokeDoesNotSupportRefOrOutParams(InvokeMethodName));

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
                Expression.Lambda<Func<T, RemoteCallerContext, IServiceProvider, Task>>(body, instanceArg,
                    httpContextArg, providerArg);

            return lambda.Compile();
        }

        private static object GetService(IServiceProvider sp, Type type, Type middleware)
        {
            var service = sp.GetService(type);
            if (service == null)
                throw new InvalidOperationException(
                    Resources.FormatException_InvokeMiddlewareNoService(type, middleware));

            return service;
        }


        internal static class Resources
        {
            private static readonly ResourceManager _resourceManager
                = new ResourceManager("Microsoft.AspNetCore.Http.Abstractions.Resources",
                    typeof(Resources).GetTypeInfo().Assembly);

            /// <summary>
            ///     '{0}' is not available.
            /// </summary>
            internal static string Exception_UseMiddlewareIServiceProviderNotAvailable =>
                GetString("Exception_UseMiddlewareIServiceProviderNotAvailable");

            /// <summary>
            ///     No public '{0}' or '{1}' method found.
            /// </summary>
            internal static string Exception_UseMiddlewareNoInvokeMethod =>
                GetString("Exception_UseMiddlewareNoInvokeMethod");

            /// <summary>
            ///     '{0}' or '{1}' does not return an object of type '{2}'.
            /// </summary>
            internal static string Exception_UseMiddlewareNonTaskReturnType =>
                GetString("Exception_UseMiddlewareNonTaskReturnType");

            /// <summary>
            ///     The '{0}' or '{1}' method's first argument must be of type '{2}'.
            /// </summary>
            internal static string Exception_UseMiddlewareNoParameters =>
                GetString("Exception_UseMiddlewareNoParameters");

            /// <summary>
            ///     Multiple public '{0}' or '{1}' methods are available.
            /// </summary>
            internal static string Exception_UseMiddleMutlipleInvokes =>
                GetString("Exception_UseMiddleMutlipleInvokes");

            /// <summary>
            ///     The path in '{0}' must start with '/'.
            /// </summary>
            internal static string Exception_PathMustStartWithSlash => GetString("Exception_PathMustStartWithSlash");

            /// <summary>
            ///     Unable to resolve service for type '{0}' while attempting to Invoke middleware '{1}'.
            /// </summary>
            internal static string Exception_InvokeMiddlewareNoService =>
                GetString("Exception_InvokeMiddlewareNoService");

            /// <summary>
            ///     The '{0}' method must not have ref or out parameters.
            /// </summary>
            internal static string Exception_InvokeDoesNotSupportRefOrOutParams =>
                GetString("Exception_InvokeDoesNotSupportRefOrOutParams");

            /// <summary>
            ///     The value must be greater than zero.
            /// </summary>
            internal static string Exception_PortMustBeGreaterThanZero =>
                GetString("Exception_PortMustBeGreaterThanZero");

            /// <summary>
            ///     No service for type '{0}' has been registered.
            /// </summary>
            internal static string Exception_UseMiddlewareNoMiddlewareFactory =>
                GetString("Exception_UseMiddlewareNoMiddlewareFactory");

            /// <summary>
            ///     '{0}' failed to create middleware of type '{1}'.
            /// </summary>
            internal static string Exception_UseMiddlewareUnableToCreateMiddleware =>
                GetString("Exception_UseMiddlewareUnableToCreateMiddleware");

            /// <summary>
            ///     Types that implement '{0}' do not support explicit arguments.
            /// </summary>
            internal static string Exception_UseMiddlewareExplicitArgumentsNotSupported =>
                GetString("Exception_UseMiddlewareExplicitArgumentsNotSupported");

            /// <summary>
            ///     Argument cannot be null or empty.
            /// </summary>
            internal static string ArgumentCannotBeNullOrEmpty => GetString("ArgumentCannotBeNullOrEmpty");

            /// <summary>
            ///     '{0}' is not available.
            /// </summary>
            internal static string FormatException_UseMiddlewareIServiceProviderNotAvailable(object p0)
            {
                return string.Format(CultureInfo.CurrentCulture,
                    GetString("Exception_UseMiddlewareIServiceProviderNotAvailable"), p0);
            }

            /// <summary>
            ///     No public '{0}' or '{1}' method found.
            /// </summary>
            internal static string FormatException_UseMiddlewareNoInvokeMethod(object p0, object p1)
            {
                return string.Format(CultureInfo.CurrentCulture, GetString("Exception_UseMiddlewareNoInvokeMethod"), p0,
                    p1);
            }

            /// <summary>
            ///     '{0}' or '{1}' does not return an object of type '{2}'.
            /// </summary>
            internal static string FormatException_UseMiddlewareNonTaskReturnType(object p0, object p1, object p2)
            {
                return string.Format(CultureInfo.CurrentCulture, GetString("Exception_UseMiddlewareNonTaskReturnType"),
                    p0, p1, p2);
            }

            /// <summary>
            ///     The '{0}' or '{1}' method's first argument must be of type '{2}'.
            /// </summary>
            internal static string FormatException_UseMiddlewareNoParameters(object p0, object p1, object p2)
            {
                return string.Format(CultureInfo.CurrentCulture, GetString("Exception_UseMiddlewareNoParameters"), p0,
                    p1, p2);
            }

            /// <summary>
            ///     Multiple public '{0}' or '{1}' methods are available.
            /// </summary>
            internal static string FormatException_UseMiddleMutlipleInvokes(object p0, object p1)
            {
                return string.Format(CultureInfo.CurrentCulture, GetString("Exception_UseMiddleMutlipleInvokes"), p0,
                    p1);
            }

            /// <summary>
            ///     The path in '{0}' must start with '/'.
            /// </summary>
            internal static string FormatException_PathMustStartWithSlash(object p0)
            {
                return string.Format(CultureInfo.CurrentCulture, GetString("Exception_PathMustStartWithSlash"), p0);
            }

            /// <summary>
            ///     Unable to resolve service for type '{0}' while attempting to Invoke middleware '{1}'.
            /// </summary>
            internal static string FormatException_InvokeMiddlewareNoService(object p0, object p1)
            {
                return string.Format(CultureInfo.CurrentCulture, GetString("Exception_InvokeMiddlewareNoService"), p0,
                    p1);
            }

            /// <summary>
            ///     The '{0}' method must not have ref or out parameters.
            /// </summary>
            internal static string FormatException_InvokeDoesNotSupportRefOrOutParams(object p0)
            {
                return string.Format(CultureInfo.CurrentCulture,
                    GetString("Exception_InvokeDoesNotSupportRefOrOutParams"), p0);
            }

            /// <summary>
            ///     The value must be greater than zero.
            /// </summary>
            internal static string FormatException_PortMustBeGreaterThanZero()
            {
                return GetString("Exception_PortMustBeGreaterThanZero");
            }

            /// <summary>
            ///     No service for type '{0}' has been registered.
            /// </summary>
            internal static string FormatException_UseMiddlewareNoMiddlewareFactory(object p0)
            {
                return string.Format(CultureInfo.CurrentCulture,
                    GetString("Exception_UseMiddlewareNoMiddlewareFactory"), p0);
            }

            /// <summary>
            ///     '{0}' failed to create middleware of type '{1}'.
            /// </summary>
            internal static string FormatException_UseMiddlewareUnableToCreateMiddleware(object p0, object p1)
            {
                return string.Format(CultureInfo.CurrentCulture,
                    GetString("Exception_UseMiddlewareUnableToCreateMiddleware"), p0, p1);
            }

            /// <summary>
            ///     Types that implement '{0}' do not support explicit arguments.
            /// </summary>
            internal static string FormatException_UseMiddlewareExplicitArgumentsNotSupported(object p0)
            {
                return string.Format(CultureInfo.CurrentCulture,
                    GetString("Exception_UseMiddlewareExplicitArgumentsNotSupported"), p0);
            }

            /// <summary>
            ///     Argument cannot be null or empty.
            /// </summary>
            internal static string FormatArgumentCannotBeNullOrEmpty()
            {
                return GetString("ArgumentCannotBeNullOrEmpty");
            }

            private static string GetString(string name, params string[] formatterNames)
            {
                var value = _resourceManager.GetString(name);

                Debug.Assert(value != null);

                if (formatterNames != null)
                    for (var i = 0; i < formatterNames.Length; i++)
                        value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");

                return value;
            }
        }
    }
}