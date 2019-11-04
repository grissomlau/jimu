using Autofac;
using Jimu.Extension;
using Jose;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jimu.Server.Auth
{
    /// <summary>
    /// support jwt middleware
    /// </summary>
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDel _next;
        private readonly JwtAuthorizationOptions _options;
        private readonly IContainer _container;
        public JwtAuthorizationMiddleware(RequestDel next, JwtAuthorizationOptions options, IContainer container)
        {
            _options = options;
            _next = next;
            _container = container;
        }

        public Task Invoke(ServiceInvokerContext context)
        {
            // get jwt token 
            if (!string.IsNullOrEmpty(_options.TokenEndpointPath)
                && context.ServiceEntry == null
                && context.RemoteInvokeMessage.ServiceId == _options.GetServiceId())
            {
                return CreateToken(context);
            }
            // jwt authentication, alse authentication the role

            if (context.ServiceEntry != null && !context.ServiceEntry.Descriptor.AllowAnonymous)
            {
                return InvokeService(context);
            }
            // service can be annoymouse request
            return _next(context);

        }

        void InvokeCheckCredential(string serviceId, JwtAuthorizationContext context)
        {
            var serviceContainer = _container.Resolve<IServiceEntryContainer>();
            var services = serviceContainer.GetServiceEntry();
            if (services.Any())
            {
                var service = services.FirstOrDefault(x => x.Descriptor.Id == serviceId);
                if (service != null)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("context", context);
                    service.Func(dic, null);
                    return;
                }
            }
            throw new EntryPointNotFoundException($"{serviceId} not found, cannot check token generate credential");
        }

        Task CreateToken(ServiceInvokerContext context)
        {
            if (string.IsNullOrEmpty(_options.CheckCredentialServiceId))
            {
                return context.Response.WriteAsync(context.TransportMessage.Id, new JimuRemoteCallResultData
                {
                    ErrorMsg = $"JwtAuthorizationOptions.CheckCredentialServiceId  must be provided",
                    ErrorCode = "500"
                });
            }
            JwtAuthorizationContext jwtAuthorizationContext = new JwtAuthorizationContext(_options, context.RemoteInvokeMessage);

            //_options.CheckCredential(jwtAuthorizationContext);
            InvokeCheckCredential(_options.CheckCredentialServiceId, jwtAuthorizationContext);
            if (jwtAuthorizationContext.IsRejected)
            {
                return context.Response.WriteAsync(context.TransportMessage.Id, new JimuRemoteCallResultData
                {
                    ErrorMsg = $"{jwtAuthorizationContext.Error}, {jwtAuthorizationContext.ErrorDescription}",
                    ErrorCode = "400"
                });
            }

            var payload = jwtAuthorizationContext.GetPayload();
            var token = JWT.Encode(payload, Encoding.ASCII.GetBytes(_options.SecretKey), JwsAlgorithm.HS256);

            var result = new ExpandoObject() as IDictionary<string, object>;
            result["access_token"] = token;
            if (_options.ValidateLifetime)
            {
                result["expired_in"] = payload["exp"];
            }

            return context.Response.WriteAsync(context.TransportMessage.Id, new JimuRemoteCallResultData
            {
                Result = result
            });
        }

        Task InvokeService(ServiceInvokerContext context)
        {

            try
            {
                var pureToken = context.RemoteInvokeMessage.Token;
                if (pureToken != null && pureToken.Trim().StartsWith("Bearer "))
                {
                    pureToken = pureToken.Trim().Substring(6).Trim();
                }
                var payload = JWT.Decode(pureToken, Encoding.ASCII.GetBytes(_options.SecretKey), JwsAlgorithm.HS256);
                var payloadObj = JimuHelper.Deserialize(payload, typeof(IDictionary<string, object>)) as IDictionary<string, object>;
                if (_options.ValidateLifetime)
                {
                    //var exp = payloadObj["exp"];
                    if (payloadObj == null || ((Int64)payloadObj["exp"]).ToDate() < DateTime.Now)
                    {
                        var result = new JimuRemoteCallResultData
                        {
                            ErrorMsg = "Token is Expired",
                            ErrorCode = "401"
                        };
                        return context.Response.WriteAsync(context.TransportMessage.Id, result);

                    }
                }
                var serviceRoles = context.ServiceEntry.Descriptor.Roles;
                if (!string.IsNullOrEmpty(serviceRoles))
                {
                    var serviceRoleArr = serviceRoles.Split(',');
                    var roles = payloadObj != null && payloadObj.ContainsKey("roles") ? payloadObj["roles"] + "" : "";
                    var authorize = roles.Split(',').Any(role => serviceRoleArr.Any(x => x.Equals(role, StringComparison.InvariantCultureIgnoreCase)));
                    if (!authorize)
                    {
                        var result = new JimuRemoteCallResultData
                        {
                            ErrorMsg = "Unauthorized",
                            ErrorCode = "401"
                        };
                        return context.Response.WriteAsync(context.TransportMessage.Id, result);
                    }
                }
                context.RemoteInvokeMessage.Payload = new JimuPayload { Items = payloadObj };
            }
            catch (Exception ex)
            {
                var result = new JimuRemoteCallResultData
                {
                    ErrorMsg = $"Token is incorrect, exception is { ex.Message}",
                    ErrorCode = "401"
                };
                return context.Response.WriteAsync(context.TransportMessage.Id, result);
            }

            return _next(context);
        }
    }
}
